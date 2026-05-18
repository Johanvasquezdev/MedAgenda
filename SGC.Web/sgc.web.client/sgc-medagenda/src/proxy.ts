import { NextRequest, NextResponse } from "next/server";

const PROTECTED_ROUTES = ["/admin", "/paciente"];

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (pathname === "/paciente/null" || pathname.startsWith("/paciente/null/")) {
    const url = request.nextUrl.clone();
    url.pathname = "/paciente/dashboard";
    return NextResponse.redirect(url);
  }

  const isProtected = PROTECTED_ROUTES.some((route) => pathname.startsWith(route));
  if (!isProtected) return NextResponse.next();

  const token = request.cookies.get("medagenda_token")?.value;
  const role = request.cookies.get("medagenda_role")?.value;

  if (!token) {
    const url = request.nextUrl.clone();
    url.pathname = "/login";
    return NextResponse.redirect(url);
  }

  if (pathname.startsWith("/admin") && role !== "Administrador") {
    const url = request.nextUrl.clone();
    url.pathname = "/login";
    return NextResponse.redirect(url);
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/admin/:path*", "/paciente/:path*"],
};
