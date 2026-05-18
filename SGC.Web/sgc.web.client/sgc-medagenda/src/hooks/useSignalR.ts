"use client";

import { useEffect } from "react";
import * as signalR from "@microsoft/signalr";

interface UseSignalROptions {
  hubUrl?: string;
  onNuevaCita?: (payload: unknown) => void;
}

export function useSignalR({ hubUrl, onNuevaCita }: UseSignalROptions) {
  useEffect(() => {
    const baseUrl =
      process.env.NEXT_PUBLIC_API_BASE_URL?.replace(/\/$/, "") ||
      process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "");
    const resolvedHubUrl = hubUrl || (baseUrl ? `${baseUrl}/citahub` : "/citahub");

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(resolvedHubUrl, {
        accessTokenFactory: () =>
          (typeof window !== "undefined" && localStorage.getItem("medagenda_token")) || "",
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    if (onNuevaCita) {
      connection.on("NuevaCita", onNuevaCita);
    }

    connection.start().catch(() => {});

    return () => {
      if (onNuevaCita) {
        connection.off("NuevaCita", onNuevaCita);
      }
      connection.stop();
    };
  }, [hubUrl, onNuevaCita]);
}
