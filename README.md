## ArtLink Exporter

Unity package for exporting bundles that can be understood by ArtLink AR App.

### Getting Started

1. Window > Package Manager > + > Install package from git URL... 
`https://github.com/ColorfulCodingLtd/ArtLinkExporter.git`

2. ArtLink Exporter > Initial Setup

3. Create your 3D object in the editor. Make sure you don't use any custom scripts. ArtLink doesn't support custom behaviours other than existing ones (TBD: CustomBehaviour JSON setup).

4. Make sure the root object of your scene is at Pos(0,0,0), Rot(0,0,0) and Scale(1,1,1).

5. Convert GameObject to Prefab: Drag and drop the root GameObject inside a folder in your Project View

6. ArtLink Exporter > ArtLink Export

7. Select the Prefab you created. Give it an asset bundle name (no spaces). Press 'Export Assets'

8. Upload the 3 generated files (inside YOUR_PROJECT/AssetBundles/...) using the Workshop Uploader web app.

### Problem Fixes

- Works on Unity 2022.3.50 (For Asset Bundle compatibility accuracy)
- Make sure prefab root is at Pos(0,0,0), Rot(0,0,0) and Scale(1,1,1)
- Color Settings - From Gamma to Linear
- Use the URP Render Asset in /Rendering/