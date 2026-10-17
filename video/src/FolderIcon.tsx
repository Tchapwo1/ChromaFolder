import React from "react";

interface FolderIconProps {
  color?: string;
  size?: number;
  glow?: boolean;
}

export const FolderIcon: React.FC<FolderIconProps> = ({
  color = "#EAB308",
  size = 120,
  glow = false,
}) => {
  // Darker shade for back plate
  const backPlateColor = color;
  
  return (
    <div
      style={{
        width: size,
        height: size,
        position: "relative",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        filter: glow ? `drop-shadow(0 0 25px ${color}88)` : "drop-shadow(0 8px 16px rgba(0,0,0,0.35))",
      }}
    >
      <svg
        width={size}
        height={size}
        viewBox="0 0 100 100"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        {/* Back Tab & Body */}
        <path
          d="M10 24C10 20.6863 12.6863 18 16 18H40C42.4 18 44.5 19.4 45.4 21.6L47.4 26.4C48.3 28.6 50.4 30 52.8 30H84C87.3137 30 90 32.6863 90 36V76C90 79.3137 87.3137 82 84 82H16C12.6863 82 10 79.3137 10 76V24Z"
          fill={backPlateColor}
          fillOpacity={0.82}
        />
        
        {/* Paper Sheet Peek */}
        <rect
          x="18"
          y="26"
          width="64"
          height="20"
          rx="3"
          fill="#F1F5F9"
          fillOpacity={0.95}
        />
        
        {/* Front Flap with Gradient */}
        <path
          d="M8 38C8 34.6863 10.6863 32 14 32H86C89.3137 32 92 34.6863 92 38V76C92 80.4183 88.4183 84 84 84H16C11.5817 84 8 80.4183 8 76V38Z"
          fill={`url(#flap-gradient-${color.replace('#', '')})`}
        />

        {/* Top Rim Optical Highlight */}
        <path
          d="M14 33H86"
          stroke="white"
          strokeOpacity={0.45}
          strokeWidth="1.2"
          strokeLinecap="round"
        />

        <defs>
          <linearGradient
            id={`flap-gradient-${color.replace('#', '')}`}
            x1="50"
            y1="32"
            x2="50"
            y2="84"
            gradientUnits="userSpaceOnUse"
          >
            <stop stopColor={color} stopOpacity={1} />
            <stop offset="1" stopColor={color} stopOpacity={0.88} />
          </linearGradient>
        </defs>
      </svg>
    </div>
  );
};
