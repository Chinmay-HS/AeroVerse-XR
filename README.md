# AeroVerse XR - By Pixel Labs

This project brings the awe-inspiring **James Webb Space Telescope (JWST)** into your physical environment using **Augmented Reality (AR)**. Built in Unity and powered by **ARCore's Ground Plane (markerless tracking)**, this experience allows users to explore the JWST model, toggle component-specific annotations, and interact with UI features like exploded view, rotation, resizing, and more.

## Logo

Now the best logo, but we think it's cool and we are happy :)

![Logo](Assets/Screenshots/Logo.jpeg)

## Features

- **Markerless AR tracking** using Unity's AR Foundation (ARCore support).
- **Interactive annotations** that display detailed information for each part of the component.
- **Component-specific UI** with buttons for:
  - Exploded View
  - Rotate
  - Resize
  - Toggle Annotations
- **Main HUD UI** with:
  - Back to Main Menu
  - Learn More (Website Button)
## Demo

Explore AeroVerse XR in action:

![Screenshot 1](Assets/Screenshots/MobileDemo2.jpeg)


## New UI Improvements Preview

#### Sign Up 

https://github.com/user-attachments/assets/6f4d14d9-b3cd-498a-bb67-d01f1b9642b7

#### Login

https://github.com/user-attachments/assets/55632ba5-ef8e-4daa-bf6e-a76788d830a2

#### Main Menu

https://github.com/user-attachments/assets/2cc49638-85e5-4360-a542-68e27554bd72

#### Watch Augmentation Testing Video

https://github.com/user-attachments/assets/0f5772da-d3b8-43b5-a229-95c5d421ec31

#### Tech Corner

![Tech Corner](https://github.com/user-attachments/assets/e9f0831a-3f99-49d5-9e4d-5062f946d398)

#### About Us

![AboutUs](https://github.com/user-attachments/assets/50eb4640-9026-4731-b3c1-4b453f064291)

### AeroVerseXR
An immersive educational platform that brings aerospace and astronomy to life through interactive 3D models in AR and VR.

## Abstract

We're building an immersive educational platform that brings aerospace and astronomy to life through interactive 3D models in AR and VR. Students can explore, annotate, and dissect models—from the James Webb Telescope, Starlink satellite, Perseverance Rover and Turbofan Engine—on any device. With real-time collaboration, TensorFlow Lite-powered quizzes, and cross-platform compatibility via OpenXR and ARCore, learning becomes visual, hands-on, and accessible anywhere.

## What's New in AeroVerseXR V2

Since launching our MVP prototype, which initially featured only the James Webb Space Telescope, we've significantly expanded the platform's capabilities and content:

### Expanded 3D Model Library
- *James Webb Space Telescope* (initial model): Enabled users to explore its structure, instruments, and deployment sequence
- *Starlink Satellite*: Added interactive elements to understand satellite arrays, orbital positioning, and communication mechanisms
- *Turbofan Engine*: Visualized airflow, combustion, and mechanical parts in motion, giving students a clear understanding of propulsion systems

### Gemini-Powered Insights
- Integrated Gemini AI to provide contextual guidance, answer questions, and surface intelligent annotations as users interact with models
- Supports deeper understanding through natural language interaction, tailored explanations, and adaptive learning pathways

### Interactive Learning and Assessment
Added quizzes powered by TensorFlow Lite, enabling:
- On-device AI for fast, responsive question generation
- Adaptive assessments based on user interaction with 3D content
- Instant feedback and progress tracking

### Cross-Platform Compatibility
Built on OpenXR for VR and ARCore for mobile AR, ensuring:
- Seamless experiences across headsets, smartphones, and tablets
- Broad device support for both immersive and accessible learning on the go

## Technology Stack

### Google Technologies
*APIs & SDKs*
- ARCore (Augmented Reality SDK)
- Google Analytics SDK (Android GPU Inspector)

*Mobile/XR Development*
- Google Play Services for AR
- OAuth 2.0 with Google

### Google Cloud Platform (GCP)
*Databases*
- Firebase Firestore

*Authentication*
- Firebase Authentication
- Identity Platform

*Analytics and Monitoring*
- Firebase Analytics (Google Analytics for Firebase)

### Game Engine and 3D Modeling
- Unity
- Blender
- C#
- ARCore

### AI and ML Technologies
- Tensorflow 
- TensorFlow Lite
- Google Gemini API (Generative AI/LLM)

## Project Setup
To get started with AeroVerse XR locally or for development, follow these steps:

### Prerequisites
- [Unity Hub](https://unity.com/download)
- **Unity Editor** (Recommended Version: `2022.3.x LTS` with AR/VR modules)
- **Android Build Support**, including:
  - Android SDK & NDK Tools
  - OpenJDK
- [Git](https://git-scm.com/)
- [Blender](https://www.blender.org/) (for 3D model adjustments, *optional*)
- Android device with **ARCore support** (for testing AR features)

### Clone the Repository

```bash
git clone https://github.com/your-username/aeroverse-xr.git
cd aeroverse-xr
```

### Open in Unity

1. Open **Unity Hub**
2. Click on **Open**
3. Select the cloned `aeroverse-xr` project directory
4. Let Unity load all assets and dependencies  
   > *Note: The first-time load may take a while depending on your system and Unity version*

```bash
AeroVerse-XR/
├── Assets/
│   ├── ARContent/               # 3D models and AR prefabs
│   ├── Scripts/                 # Core C# scripts
│   ├── UI/                      # UI canvases and prefabs
│   ├── Resources/               # Annotation data, textures
│   ├── Scenes/                  # Unity scenes (MainMenu, ARScene, etc.)
│   └── ...
├── Packages/                    # Unity packages
├── ProjectSettings/             # Unity project settings
└── README.md
```

### Building for Android (ARCore)

1. Go to **`File > Build Settings`**
2. Select **Android** and click **Switch Platform**
3. Under **Player Settings**, configure the following:

   - **Minimum API Level**: Android 7.0 (API level 24)
   - **AR Support**: Enable **ARCore**
   - **XR Plug-in Management**: Add **ARCore** under XR Plug-in Management
   - **Package Name**: `com.pixel.aeroversexr` *(or your own unique identifier)*

4. Connect your **ARCore-supported Android device** with **USB debugging enabled**
5. Click **Build and Run** to deploy the app

---

### Firebase Setup

This project uses Firebase for:

- **Authentication**
- **Firestore Database**
- **Storage** (for model uploads or user data)

#### Steps to Set Up Firebase

1. Go to the [Firebase Console](https://console.firebase.google.com/)
2. Create a new project or use an existing one
3. Download the `google-services.json` file
4. Place the file in the following path of your Unity project: Assets/Plugins/Android/

#### Enable the Following Firebase Services

- Firebase Authentication  
- Firestore Database  
- Firebase Storage  

#### Install Firebase SDKs

Install Firebase SDKs in Unity using one of the following methods:

- **Unity Package Manager** *(if configured for scoped Firebase registries)*  
- **Firebase Unity SDK**: [Download here](https://firebase.google.com/docs/unity/setup) and import the relevant `.unitypackage` files

> Make sure to restart Unity after importing Firebase packages to avoid initialization issues.

## Usage Instructions

### Getting Started
1. *Download and Install*: Available on supported platforms (Android, iOS, VR headsets)
2. *Sign In / Register*: Create an account or sign in using credentials

### Navigating the Interface
- *Model Library*: Browse available 3D models
- *Collaboration Hub*: Join or create real-time collaborative sessions
- *Quizzes & Lessons*: Access interactive lessons and assessments
- *Settings*: Adjust accessibility and display options

### Interacting with 3D Models
- *Load a Model*: Tap to load any 3D model
- *Navigation*: Rotate, zoom, pan using touch or VR controllers
- *Annotate*: Tap on components to view or add notes
- *Explode/Reassemble*: Separate components to view inner details/

### Working with Annotations

- Annotation labels are **world-space UI prefabs** that appear above specific parts of the telescope.
- These annotations face the camera and toggle on/off via a UI button.
- Annotations are created and linked using the `AnnotationManager.cs` script. 

The script automatically:
- Instantiates labels at attach points
- Makes them face the user
- Toggles them with a single button
- Displays console logs for debugging


## Tips for Best Experience

- *Canvas Scaling*: Set your world-space canvas scale to 0.01 for AR readability
- *Device Compatibility*: Ensure ARCore support is enabled in Project Settings
- *Testing*: Test on a flat surface with proper lighting for optimal ARCore plane detection
- Use AR in well-lit areas with clear flat surfaces
- For VR, ensure your headset is fully charged and calibrated
- Keep internet connection stable for real-time features

## Important Links

- *Demo Video*: [YouTube Link](https://youtu.be/lVXMZh7JIX4)
- *Figma Design*: [UI/UX Design](https://www.figma.com/design/7ZlWlTToGLMatbPIIHzUd4/GDG-H2S-2025-UI-UX?node-id=9-2&t=3liSpuhg2E3nmuAr-1)
- *VR Repository*: [AeroVerse XR VR Repo](link)
- *Medium Articles*: [Technical Blogs](https://medium.com/@pixellabs010)
- *Website*: [aeroverse-xr.vercel.app](https://aeroverse-xr.vercel.app)

## 3D Model Attribution

We gratefully acknowledge the following 3D model sources:

- *Turbofan Engine*: [Sketchfab Model](https://sketchfab.com/3d-models/turbine-turbofan-engine-jet-engine-74c6aceed86b4a41aaad3b93afc3e262)
- *Starlink SpaceX Satellite*: [Sketchfab Model](https://sketchfab.com/3d-models/starlink-spacex-satellite-0a60f6720c5141c9a1c6d71aac108b31)
- *JWST*: [NASA 3D Model](https://nasa3d.arc.nasa.gov/search/james%20webb%20/model?fbclid=IwAR1AhiYr1UYTTFXql6noCEpyC5iIGMXCwusCPHZXe2Xttr9PEptsNpg49mQ)
- *Perseverance Rover*: [Sketchfab Model](https://sketchfab.com/3d-models/perseverance-mars-rover-53034dff0df04f698f2a5d532366adc4)

## Acknowledgements

We gratefully acknowledge the contributions of:

- *NASA (National Aeronautics and Space Administration)*: For providing open access to scientific data, 3D models, and educational resources
- *ESA (European Space Agency)*: For their valuable public data, mission assets, and commitment to advancing space science and education
- *3D Artists & Open Source Contributors*: Thank you to the talented 3D modellers and creative communities whose work helps bring aerospace and astronomy concepts to life

## Team Pixel Labs

The minds behind Aeroverse XR, Pixel Labs are just a group of friends trying to make a difference with their individual skills:

- *Aryan Yadav*: Software Developer and AI Engineer
  - [Portfolio](https://aryanyadav-portfolio.vercel.app) | [LinkedIn](https://www.linkedin.com/in/-aryanyadav/)

- *Chinmay Sawant*: VR Developer, XR specialist and Editor
  - [LinkedIn](https://www.linkedin.com/in/chinmay-sawant-8b3282266/) | [YouTube](https://www.youtube.com/c/chinmayhs)

- *Om Awadhoot*: AR Developer, XR Generalist and Backend Developer
  - [LinkedIn](https://www.linkedin.com/in/om-awadhoot/)

- *Diya Pagdhal*: AR Developer and Content Writer
  - [LinkedIn](https://www.linkedin.com/in/diya-paghdal-630951271/)

## Support

For technical support, bug reports, or feedback, visit our website at [aeroverse-xr.vercel.app](https://aeroverse-xr.vercel.app) and use the contact form.
