import { registerRoot, Composition } from "remotion";
import React from "react";
import { FolderColorizerTeaser } from "./FolderColorizerTeaser";

export const RemotionRoot: React.FC = () => {
  return React.createElement(Composition, {
    id: "FolderColorizerTeaser",
    component: FolderColorizerTeaser,
    durationInFrames: 360,
    fps: 30,
    width: 1920,
    height: 1080,
  });
};

registerRoot(RemotionRoot);
