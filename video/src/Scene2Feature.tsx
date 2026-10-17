import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { FolderIcon } from "./FolderIcon";

export const Scene2Feature: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Entrance spring for main showcase
  const mainEntrance = spring({
    frame,
    fps,
    config: { damping: 14, mass: 0.6, stiffness: 120 },
  });

  // Click trigger happens at frame 26
  const isClicked = frame >= 26;
  
  // Shockwave ring after click
  const shockwaveProgress = interpolate(frame, [26, 60], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  const shockwaveScale = interpolate(shockwaveProgress, [0, 1], [0.8, 3.0]);
  const shockwaveOpacity = interpolate(shockwaveProgress, [0, 1], [0.85, 0]);

  // Folder scale bounce on color transform
  const bounceSpring = spring({
    frame: frame - 26,
    fps,
    config: { damping: 10, mass: 0.5, stiffness: 180 },
  });
  const folderScale = isClicked ? interpolate(bounceSpring, [0, 1], [1.35, 1.1]) : 1.0;

  // Context menu entrance (frame 6 to 18) and exit (frame 28 to 36)
  const menuOpacity = interpolate(frame, [6, 16, 26, 36], [0, 1, 1, 0], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });
  const menuY = interpolate(frame, [6, 16], [15, 0], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  // Palette cards staggered fan-out (starts at frame 38)
  const colorsData = [
    { name: "Red", hex: "#DC2626", label: "Urgent" },
    { name: "Blue", hex: "#2563EB", label: "Active Project" },
    { name: "Green", hex: "#16A34A", label: "Completed" },
    { name: "Yellow", hex: "#EAB308", label: "In Review" },
    { name: "Orange", hex: "#EA580C", label: "High Priority" },
    { name: "Purple", hex: "#9333EA", label: "Personal" },
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
      }}
    >
      {/* Dynamic Background Ambient Glow */}
      <div
        style={{
          position: "absolute",
          width: 800,
          height: 800,
          borderRadius: "50%",
          background: isClicked
            ? "radial-gradient(circle, rgba(37, 99, 235, 0.30) 0%, rgba(11, 15, 25, 0) 70%)"
            : "radial-gradient(circle, rgba(234, 179, 8, 0.15) 0%, rgba(11, 15, 25, 0) 70%)",
          filter: "blur(90px)",
        }}
      />

      {/* Top Headline */}
      <div
        style={{
          position: "absolute",
          top: 60,
          textAlign: "center",
          opacity: mainEntrance,
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
            marginBottom: 8,
          }}
        >
          Right-Click. Transform in Seconds.
        </div>
        <div style={{ fontSize: 20, color: "#64748B", fontWeight: 500 }}>
          Directly from native Windows 11 File Explorer.
        </div>
      </div>

      {/* Central Interactive Transform Area */}
      <div
        style={{
          position: "relative",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          marginTop: -40,
        }}
      >
        {/* Shockwave circle */}
        {isClicked && (
          <div
            style={{
              position: "absolute",
              width: 220,
              height: 220,
              borderRadius: "50%",
              border: "3px solid #3B82F6",
              transform: `scale(${shockwaveScale})`,
              opacity: shockwaveOpacity,
              pointerEvents: "none",
            }}
          />
        )}

        {/* Center Folder */}
        <div
          style={{
            transform: `scale(${folderScale})`,
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            gap: 12,
          }}
        >
          <FolderIcon
            color={isClicked ? "#2563EB" : "#EAB308"}
            size={180}
            glow={isClicked}
          />
          <span
            style={{
              fontSize: 22,
              fontWeight: 600,
              color: isClicked ? "#93C5FD" : "#CBD5E1",
              letterSpacing: "0.02em",
            }}
          >
            {isClicked ? "Client_Deliverables" : "Important_Folder"}
          </span>
        </div>

        {/* Windows 11 Fluent Context Menu Mockup */}
        <div
          style={{
            position: "absolute",
            top: -20,
            left: 210,
            width: 270,
            backgroundColor: "rgba(30, 41, 59, 0.90)",
            backdropFilter: "blur(24px)",
            border: "1px solid rgba(255, 255, 255, 0.15)",
            borderRadius: 14,
            boxShadow: "0 25px 50px rgba(0, 0, 0, 0.6)",
            padding: "10px 8px",
            opacity: menuOpacity,
            transform: `translateY(${menuY}px)`,
            zIndex: 30,
          }}
        >
          <div
            style={{
              padding: "6px 12px",
              fontSize: 13,
              fontWeight: 700,
              color: "#94A3B8",
              borderBottom: "1px solid rgba(255,255,255,0.08)",
              marginBottom: 6,
            }}
          >
            Folder Colorizer
          </div>
          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: 12,
              padding: "8px 12px",
              borderRadius: 8,
              backgroundColor: frame >= 22 ? "rgba(37, 99, 235, 0.4)" : "transparent",
              color: "#FFFFFF",
              fontSize: 14,
              fontWeight: 600,
            }}
          >
            <span style={{ width: 12, height: 12, borderRadius: "50%", backgroundColor: "#2563EB" }} />
            <span>Blue (Active)</span>
            {frame >= 24 && <span style={{ marginLeft: "auto", fontSize: 13, color: "#60A5FA" }}>✓</span>}
          </div>
          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: 12,
              padding: "8px 12px",
              borderRadius: 8,
              color: "#CBD5E1",
              fontSize: 14,
            }}
          >
            <span style={{ width: 12, height: 12, borderRadius: "50%", backgroundColor: "#DC2626" }} />
            <span>Red (Urgent)</span>
          </div>
          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: 12,
              padding: "8px 12px",
              borderRadius: 8,
              color: "#CBD5E1",
              fontSize: 14,
            }}
          >
            <span style={{ width: 12, height: 12, borderRadius: "50%", backgroundColor: "#16A34A" }} />
            <span>Green (Done)</span>
          </div>
        </div>
      </div>

      {/* Centered Palette Showcase Cards */}
      <div
        style={{
          position: "absolute",
          bottom: 55,
          left: 0,
          right: 0,
          display: "flex",
          justifyContent: "center",
          gap: 20,
        }}
      >
        {colorsData.map((c, idx) => {
          const cardSpring = spring({
            frame: frame - (36 + idx * 4),
            fps,
            config: { damping: 14, mass: 0.5, stiffness: 140 },
          });
          const cardScale = interpolate(cardSpring, [0, 1], [0.75, 1]);
          const cardOpacity = interpolate(cardSpring, [0, 1], [0, 1]);

          return (
            <div
              key={idx}
              style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                backgroundColor: "rgba(30, 41, 59, 0.75)",
                border: "1px solid rgba(255, 255, 255, 0.12)",
                borderRadius: 16,
                padding: "16px 20px",
                width: 140,
                transform: `scale(${cardScale})`,
                opacity: cardOpacity,
                boxShadow: "0 10px 25px rgba(0, 0, 0, 0.35)",
              }}
            >
              <FolderIcon color={c.hex} size={54} />
              <span style={{ fontSize: 15, fontWeight: 700, color: "#FFFFFF", marginTop: 8 }}>
                {c.name}
              </span>
              <span style={{ fontSize: 12, fontWeight: 500, color: "#94A3B8" }}>
                {c.label}
              </span>
            </div>
          );
        })}
      </div>
    </AbsoluteFill>
  );
};
