"""Run with Python, numpy, playwright (chromium installed), imageio-ffmpeg."""
from base64 import b64decode
from functools import partial
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path
import subprocess
from threading import Thread
import imageio_ffmpeg
from playwright.sync_api import sync_playwright
from make_audio import compose, ROOT, BEATS, BEAT

FPS = 60


def render():
    compose()
    with ThreadingHTTPServer(('127.0.0.1', 0), partial(SimpleHTTPRequestHandler, directory=str(ROOT))) as server:
        Thread(target=server.serve_forever, daemon=True).start()
        with sync_playwright() as pw:
            browser = pw.chromium.launch()
            page = browser.new_page()
            page.goto(f'http://127.0.0.1:{server.server_port}/index.html')
            page.evaluate('() => window.assetsReady')
            encode = [imageio_ffmpeg.get_ffmpeg_exe(), '-y','-loglevel','error',
                      '-f','image2pipe','-framerate',str(FPS),'-vcodec','mjpeg','-i','pipe:0',
                      '-i',str(ROOT/'score.wav'),'-c:v','libx264','-preset','veryfast','-crf','18',
                      '-maxrate','8M','-bufsize','12M','-pix_fmt','yuv420p','-c:a','aac','-b:a','160k',
                      '-movflags','+faststart',str(ROOT/'Ocho-y-media.mp4')]
            with subprocess.Popen(encode, stdin=subprocess.PIPE) as proc:
                for frame in range(round(BEATS*BEAT*FPS)):
                    jpg = page.evaluate("""([i,f]) => {
                        renderFrame(i,f);
                        return document.querySelector('canvas').toDataURL('image/jpeg',.94).split(',')[1];
                    }""", [frame,FPS])
                    proc.stdin.write(b64decode(jpg))
                proc.stdin.close()
                if proc.wait():
                    raise RuntimeError('FFmpeg could not finish the movie')
            browser.close()
        server.shutdown()


if __name__ == '__main__':
    render()
