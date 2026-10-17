import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";

export const Scene3Pillars: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  const titleSpring = spring({
    frame,
    fps,
    config: { damping: 14, mass: 0.6, stiffness: 120 },
  });

  const titleOpacity = interpolate(titleSpring, [0, 1], [0, 1]);
  const titleY = interpolate(titleSpring, [0, 1], [25, 0]);

  const pillars = [
    {
      icon: "⚡",
      title: "Instant Shell Redraw",
      subtitle: "Win32 SHChangeNotify cache flush. No Explorer restart required.",
      accent: "#3B82F6",
    },
    {
      icon: "🔄",
      title: "1-Click Reversible",
      subtitle: "Restore default Windows appearance instantly without touching contents.",
      accent: "#10B981",
    },
    {
      icon: "🔒",
      title: "100% Offline & Private",
      subtitle: "Zero network calls, zero telemetry, and zero background daemons.",
      accent: "#8B5CF6",
    },
    {
      icon: "🛡️",
      title: "Zero Admin Required",
      subtitle: "Installs and runs entirely in user-space (HKCU). Never triggers UAC.",
      accent: "#F59E0B",
    },
  ];

  return (
    <AbsoluteFill
      style={{
        backgroundColor: "#0B0F19",
        color: "#F8FAFC",
        fontFamily: "system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        overflow: "hidden",
        padding: "60px 100px",
      }}
    >
      {/* Background ambient lighting */}
      <div
        style={{
          position: "absolute",
          width: 800,
          height: 800,
          borderRadius: "50%",
          background: "radial-gradient(circle, rgba(59, 130, 246, 0.15) 0%, rgba(11, 15, 25, 0) 70%)",
          filter: "blur(90px)",
        }}
      />

      {/* Header */}
      <div
        style={{
          textAlign: "center",
          marginBottom: 60,
          opacity: titleOpacity,
          transform: `translateY(${titleY}px)`,
        }}
      >
        <div
          style={{
            fontSize: 48,
            fontWeight: 800,
            letterSpacing: "-0.03em",
            background: "linear-gradient(180deg, #FFFFFF 0%, #94A3B8 100%)",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
            marginBottom: 10,
          }}
        >
          Built for Speed. Designed for Safety.
        </div>
        <div style={{ fontSize: 22, color: "#94A3B8", fontWeight: 500 }}>
          Feels like a native Windows 11 feature from day one.
        </div>
      </div>

      {/* 2x2 Pillar Cards Grid */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(2, 1fr)",
          gap: 28,
          maxWidth: 1080,
          width: "100%",
        }}
      >
        {pillars.map((p, idx) => {
          const cardSpring = spring({
            frame: frame - (15 + idx * 6),
            fps,
            config: { damping: 14, mass: 0.6, stiffness: 120 },
          });

          const scale = interpolate(cardSpring, [0, 1], [0.85, 1]);
          const opacity = interpolate(cardSpring, [0, 1], [0, 1]);
          const y = interpolate(cardSpring, [0, 1], [20, 0]);

          return (
            <div
              key={idx}
              style={{
                backgroundColor: "rgba(30, 41, 59, 0.5)",
                border: "1px solid rgba(255, 255, 255, 0.08)",
                borderRadius: 18,
                padding: "28px 32px",
                display: "flex",
                alignItems: "flex-start",
                gap: 20,
                transform: `scale(${scale}) translateY(${y}px)`,
                opacity,
                backdropFilter: "blur(12px)",
              }}
            >
              <div
                style={{
                  fontSize: 36,
                  width: 60,
                  height: 60,
                  borderRadius: 14,
                  backgroundColor: `${p.accent}22`,
                  border: `1px solid ${p.accent}44`,
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  flexShrink: 0,
                }}
              >
                {p.icon}
              </div>
              <div style={{ display: "flex", flexDirection: "column", gap: 6 }}>
                <span style={{ fontSize: 22, fontWeight: 700, color: "#FFFFFF" }}>
                  {p.title}
                </span>
                <span style={{ fontSize: 16, color: "#94A3B8", lineHeight: 1.4 }}>
                  {p.subtitle}
                </span>
              </div>
            </div>
          );
        })}
      </div>
    </AbsoluteFill>
  );
};
