# Uploaded Texture Archive Inventory

Uploaded archive inspected:

```text
Download(1).7z
Size: about 50 MB
Format: 7-Zip archive
```

The archive contains these material packs:

```text
Fabric028_1K-JPG.zip
Leaking005_1K-JPG.zip
Leaking012A_1K-JPG.zip
Leaking013C_1K-JPG.zip
Metal028_1K-JPG.zip
OfficeCeiling001_1K-JPG.zip
OfficeCeiling002_1K-JPG.zip
PaintedMetal001_1K-JPG.zip
PaintedPlasterSubstance001_COMPILED.sbsar
Plastic005_1K-JPG.zip
Wood026_1K-JPG.zip
```

## Recommended in-game mapping

| Game surface | Material pack | Primary maps |
|---|---|---|
| Main carpet | `Fabric028_1K-JPG.zip` | Color, NormalGL, Roughness |
| Wall leaks / grime decals | `Leaking005_1K-JPG.zip` | Color, Opacity, NormalGL |
| Extra leak variants | `Leaking012A`, `Leaking013C` | Color, NormalGL, Roughness |
| Office ceiling tiles | `OfficeCeiling001` or `OfficeCeiling002` | Color, NormalGL, Roughness, Emission |
| Metal service doors | `PaintedMetal001` or `Metal028` | Color, Metalness, Roughness, NormalGL |
| Plastic light covers | `Plastic005` | Color, NormalGL, Roughness |
| Wood trim / old boards | `Wood026` | Color, NormalGL, Roughness |
| Procedural plaster source | `PaintedPlasterSubstance001_COMPILED.sbsar` | Substance material source |

## Why the raw archive should not be committed directly

The uploaded archive is about 50 MB. For this repo, large binary assets should be added through Git LFS, Releases, or a repo-side asset workflow instead of the GitHub Contents API.

## Safe next implementation path

1. Extract the archive locally.
2. Export game-ready maps at 512 or 1024 resolution.
3. Keep only the maps Godot needs:
   - `*_Color`
   - `*_NormalGL`
   - `*_Roughness`
   - `*_Opacity` for decals
   - `*_Emission` only for light panels
4. Add them under:

```text
godot/assets/textures/uploaded/
```

5. Update Godot materials to load these paths.
6. Keep the full original archive outside git or attach it as a Release/LFS asset.
