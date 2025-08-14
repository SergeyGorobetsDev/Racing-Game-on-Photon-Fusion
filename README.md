# Cart

# Setup
 - Firebase Setup (This guide explains how to set up Firebase in a Unity project. Since Firebase SDK files and configs are excluded from Git (via .gitignore), follow these steps to configure Firebase properly.)
   - 🚀 Firebase Setup Steps
      - Add Firebase to Your Unity Project
      - Option A: Unity Package Manager (Recommended)
      - Open Unity and go to: Window > Package Manager > Click "+" > "Add package from Git URL" > Paste the Firebase SDK URL (e.g., for Authentication & Firestore)
  - 🚀 Firebase Unity SDK Download
    - Import the .unitypackage files you need (e.g., FirebaseAuth.unitypackage, FirebaseFirestore.unitypackage).
    - Configure Firebase for Your Platform
      - Android Setup
        - In Firebase Console:
        - Go to Project Settings > General > Your Apps
        - Click "Add App" > Android
        - Enter your Package Name (e.g., com.yourcompany.game)
        - Download google-services.json
      - Place google-services.json in: Assets/Plugins/Android/
