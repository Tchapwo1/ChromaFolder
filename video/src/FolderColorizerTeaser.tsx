import React from "react";
import { AbsoluteFill, Sequence } from "remotion";
import { Scene1Hook } from "./Scene1Hook";
import { Scene2Feature } from "./Scene2Feature";
import { Scene3Pillars } from "./Scene3Pillars";
import { Scene4Outro } from "./Scene4Outro";

export const FolderColorizerTeaser: React.FC = () => {
  return (
    <AbsoluteFill style={{ backgroundColor: "#0B0F19" }}>
      {/* Scene 1: The Problem Hook (0 - 90 frames / 0s - 3s) */}
      <Sequence from={0} durationInFrames={90}>
        <Scene1Hook />
      </Sequence>

      {/* Scene 2: The Magic Feature & Palette (90 - 200 frames / 3s - 6.67s) */}
      <Sequence from={90} durationInFrames={110}>
        <Scene2Feature />
      </Sequence>

      {/* Scene 3: Native Performance & Security Pillars (200 - 290 frames / 6.67s - 9.67s) */}
      <Sequence from={200} durationInFrames={90}>
        <Scene3Pillars />
      </Sequence>

      {/* Scene 4: Grand Slam Brand Outro & CTA (290 - 360 frames / 9.67s - 12s) */}
      <Sequence from={290} durationInFrames={70}>
        <Scene4Outro />
      </Sequence>
    </AbsoluteFill>
  );
};
