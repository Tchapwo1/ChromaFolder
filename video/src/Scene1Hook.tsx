import React from "react";
import {
  AbsoluteFill,
  interpolate,
  spring,
  useCurrentFrame,
  useVideoConfig,
} from "remotion";
import { FolderIcon } from "./FolderIcon";

export const Scene1Hook: React.FC = () => {
  const frame = useCurrentFrame();
  const { fps } = useVideoConfig();

  // Grid fade-in and subtle zoom
  const gridEntrance = spring({
    frame,
    fps,
    config: { damping: 16, mass: 0.8, stiffness: 80 },
  });

  const gridScale = interpolate(frame, [0, 90], [0.95, 1.05], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  // Typography spring
  const textSpring = spring({
    frame: frame - 15,
    fps,
    config: { damping: 12, mass: 0.5, stiffness: 140 },
  });

  const textOpacity = interpolate(textSpring, [0, 1], [0, 1]);
  const textY = interpolate(textSpring, [0, 1], [30, 0]);

  // Subtitle fade
  const subOpacity = interpolate(frame, [35, 55], [0, 1], {
    extrapolateLeft: "clamp",
    extrapolateRight: "clamp",
  });

  // Folder names simulating a cluttered workspace
  const folderNames = [
    "Documents", "Downloads", "Project_Alpha",
    "Finance_2026", "Archives", "Invoices",
    "Client_Beta", "Taxes", "Temp_Backups"
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
      {/* Background radial atmosphere */}
      <div
        style={{
          position: "absolute",
          width: 800,
          height: 800,
          borderRadius: "50%",
          background: "radial-gradient(circle, rgba(234, 179, 8, 0.12) 0%, rgba(11, 15, 25, 0) 70%)",
          filter: "blur(60px)",
        }}
      />

      {/* Identical Yellow Folders Grid */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(3, 1fr)",
          gap: "28px 48px",
          transform: `scale(${gridScale * gridEntrance})`,
          opacity: gridEntrance,
          marginBottom: 40,
        }}
      >
        {folderNames.map((name, i) => (
          <div
            key={i}
            style={{
              display: "flex",
              flexDirection: "column",
              alignItems: "center",
              gap: 8,
            }}
          >
            <FolderIcon color="#EAB308" size={88} />
            <span
              style={{
                fontSize: 14,
                fontWeight: 500,
                color: "#94A3B8",
                letterSpacing: "0.02em",
              }}
            >
              {name}
            </span>
          </div>
        ))}
      </div>

      {/* Kinetic Typography */}
      <div
        style={{
          position: "absolute",
          bottom: 70,
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
            fontSize: 48,
            fontWeight: 800,
            letterSpacing: "-0.03em",
            background: "linear-gradient(180deg, #FFFFFF 0%, #CBD5E1 100%)",
            WebkitBackgroundClip: "text",
            WebkitTextFillColor: "transparent",
            marginBottom: 8,
          }}
        >
          Lost in a sea of identical yellow folders?
        </div>
        <div
          style={{
            fontSize: 22,
            fontWeight: 500,
            color: "#94A3B8",
            opacity: subOpacity,
          }}
        >
          Important projects shouldn't blend into the background.
        </div>
      </div>
    </AbsoluteFill>
  );
};
