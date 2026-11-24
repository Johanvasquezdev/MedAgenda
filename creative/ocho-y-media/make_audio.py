"""Original 16-second pencil-and-synth cue, 120 BPM."""
from pathlib import Path
import wave
import numpy as np

RATE, BPM, BEATS = 48000, 120, 32
BEAT = 60 / BPM
ROOT = Path(__file__).resolve().parent


def compose():
    rng = np.random.default_rng(830)
    mix = np.zeros(round(RATE * BEAT * BEATS))

    def add(beat, sound):
        start = round(beat * BEAT * RATE)
        count = min(len(sound), len(mix) - start)
        mix[start:start + count] += sound[:count]

    def time(duration):
        return np.arange(round(RATE * duration)) / RATE

    # Dry pencil taps and a brushed-paper texture before the agenda opens.
    for beat in [0, 1.5, 3, 4.5, 6, 7, 8.5]:
        t = time(.14)
        add(beat, .22 * np.exp(-t * 46) *
            (np.sin(2*np.pi*850*t) + .5*np.sin(2*np.pi*1430*t)))
    for beat in [2, 5, 8.75]:
        t = time(.28)
        add(beat, .042*rng.normal(size=t.size)*np.sin(np.pi*t/.28)**2)
    for beat in range(10, BEATS):
        root = [146.83, 174.61, 130.81, 146.83][((beat-10)//4) % 4]
        if beat % 2 == 0:
            t = time(.28)
            add(beat, .45*np.exp(-t*15)*np.sin(2*np.pi*(54*t+2.8*(1-np.exp(-t*28)))))
        else:
            t = time(.14)
            add(beat, .15*np.exp(-t*32)*rng.normal(size=t.size))
        t = time(.07)
        add(beat+.5, .045*np.diff(rng.normal(size=t.size+1))*np.exp(-t*60))
        if (beat-10) % 4 == 0:
            t = time(1.92)
            pad = np.zeros(t.size)
            for interval in [1, 1.5, 2.25]:
                for detune in np.linspace(.995, 1.005, 7):
                    phase = t*root*interval*detune
                    pad += 2*(phase % 1)-1
            duck = .42+.58*(1-np.exp(-8*(t % 1)))
            add(beat, .007*pad*duck*np.minimum(t/.12,1)*np.minimum((1.92-t)/.3,1))
    for beat, freq in [(10,587.33),(10.5,880),(22,587.33),(23,880),(28,1174.66)]:
        t = time(.85)
        add(beat, .14*np.sin(2*np.pi*freq*t)*np.exp(-6*t)*np.minimum(1,t/.006))
    drop = round(10 * BEAT * RATE)
    mix[drop-round(.10*RATE):drop] = 0
    mix[-RATE:] *= np.linspace(1,0,RATE)
    with wave.open(str(ROOT / 'score.wav'), 'wb') as out:
        out.setparams((1,2,RATE,0,'NONE','not compressed'))
        out.writeframes((np.tanh(mix)*30000).astype('<i2').tobytes())


if __name__ == '__main__':
    compose()
