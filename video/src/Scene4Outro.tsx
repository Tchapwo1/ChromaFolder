import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { FolderIcon } from "./FolderIcon";

export const Scene4Outro: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Snappy hero entrance
  const heroEntrance = spring({
    frame,
    fps,
    config: { damping: 12, mass: 0.5, stiffness: 130 },
  });

  const heroScale = interpolate(heroEntrance, [0, 1], [0.75, 1]);
  const heroOpacity = interpolate(heroEntrance, [0, 1], [0, 1]);

  // Subtle floating motion
  const floatY = Math.sin(frame / 12) * 8;

  // Text entrance (starts frame 12)
  const textSpring = spring({
    frame: frame - 12,
    fps,
    config: { damping: 14, mass: 0.6, stiffness: 120 },
  });
  const textOpacity = interpolate(textSpring, [0, 1], [0, 1]);
  const textY = interpolate(textSpring, [0, 1], [20, 0]);

  // Badge entrance (starts frame 25)
  const badgeSpring = spring({
    frame: frame - 25,
    fps,
    config: { damping: 12, mass: 0.5, stiffness: 150 },
  });
  const badgeScale = interpolate(badgeSpring, [0, 1], [0.8, 1]);
  const badgeOpacity = interpolate(badgeSpring, [0, 1], [0, 1]);

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
      }}
    >
      {/* Dynamic Multi-Color Background Glow */}
      <div
        style={{
          position: "absolute",
          width: 750,
          height: 750,
          borderRadius: "50%",
          background: "radial-gradient(circle, rgba(37, 99, 235, 0.25) 0%, rgba(147, 51, 234, 0.15) 40%, rgba(11, 15, 25, 0) 70%)",
          filter: "blur(90px)",
          transform: `scale(${1 + Math.sin(frame / 15) * 0.05})`,
        }}
      />

      {/* Central Hero Folder */}
      <div
        style={{
          transform: `scale(${heroScale}) translateY(${floatY}px)`,
          opacity: heroOpacity,
          marginBottom: 30,
        }}
      >
        <FolderIcon color="#2563EB" size={170} glow={true} />
      </div>

      {/* Product Title & Brand Hook */}
      <div
        style={{
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          textAlign: "center",
          opacity: textOpacity,
          transform: `translateY(${textY}px)`,
        }}
      >
        <div
          style={{
            fontSize: 64,
            fontWeight: 900,
            letterSpacing: "-0.04em",
            background: "linear-gradient(180deg, #FFFFFF 0%, #E2E8F0 100%)",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
            marginBottom: 10,
          }}
        >
          Folder Colorizer
        </div>
        <div
          style={{
            fontSize: 24,
            fontWeight: 600,
            color: "#60A5FA",
            letterSpacing: "-0.01em",
            marginBottom: 36,
          }}
        >
          Visual clarity for Windows 11 File Explorer.
        </div>
      </div>

      {/* Grand Slam CTA Badge */}
      <div
        style={{
          display: "flex",
          alignItems: "center",
          gap: 16,
          backgroundColor: "rgba(30, 41, 59, 0.7)",
          border: "1px solid rgba(255, 255, 255, 0.15)",
          borderRadius: 40,
          padding: "14px 32px",
          transform: `scale(${badgeScale})`,
          opacity: badgeOpacity,
          boxShadow: "0 10px 30px rgba(0, 0, 0, 0.4)",
        }}
      >
        <span style={{ fontSize: 18, color: "#10B981" }}>●</span>
        <span style={{ fontSize: 18, fontWeight: 700, color: "#FFFFFF" }}>
          Lightweight &bull; 100% Offline &bull; Free MVP
        </span>
      </div>
    </AbsoluteFill>
  );
};
