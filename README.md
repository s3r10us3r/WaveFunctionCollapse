# Wave Function Collapse (WFC)

This repository contains a **Wave Function Collapse** algorithm implementation in **.NET 8** with a **GUI built in [Avalonia UI](https://avaloniaui.net/)**. The application allows you to provide an input image, tweak various settings, and observe procedural image generation in real time.

## What is Wave Function Collapse?
**Wave Function Collapse (WFC)** is a procedural content generation algorithm originally created by [@mxgmn](https://github.com/mxgmn). It takes a small input image and produces larger images that are locally similar to the input. The algorithm does so by:
- Extracting *patterns* of size \(n \times n\) from the input.
- Ensuring that these patterns appear in the output with the same frequency and adjacency constraints as in the original image.

---

## Features
- **GUI-based**: An intuitive interface to load an image and tweak parameters.
- **Pattern Size (N)**: Configure the size of the patterns (in pixels).
- **Seed**: Set a specific seed for reproducible results or leave it blank for randomness.
- **Rotations & Reflections**: Choose to allow rotated or reflected patterns in the output.
- **Width & Height**: Specify the desired output resolution (in pixels).
- **Locks**: Constrain certain edges (e.g., top, bottom) of the output to match edges from the input.

---

## Algorithm Explanation
The WFC algorithm enforces two main constraints on the generated bitmap:
1. Any \(n \times n\) region in the output *must* appear in the input (unless you enable reflection or rotation).  
2. Each \(n \times n\) region in the output appears with roughly the same probability it has in the input.

In other words, we produce a new image that’s **visually similar** to the input, while not being an exact copy.

For more details, visit the original project:  
[**Wave Function Collapse**](https://github.com/mxgmn/WaveFunctionCollapse)

---

## Usage
1. **N**: Size of the patterns in pixels. Higher values produce less randomness (but may cause slow performance or failed generation if too large, e.g., \(N > 4\)).
2. **Seed**: Enter a specific number to reproduce the same output, or leave it blank for a random seed.
3. **Rotations**: If enabled, the output can include patterns rotated \(90^\circ, 180^\circ, 270^\circ\).
4. **Reflections**: If enabled, the output can include patterns mirrored along the y-axis.
5. **Width & Height**: Dimensions (in pixels) of the output image.
6. **Locks**: Constrain edges so the output inherits edge patterns from the input. (Helpful for always keeping certain elements like a “ground” or “sky” region.)

---

## Examples

### Input Image
Here is an example input image:  
![input-image](https://github.com/user-attachments/assets/0f467e57-84e6-4823-9807-2226143ae202)

### Small Pattern Size (N=2)
With \(N = 2\), the resulting image barely resembles the original flowers:  
![output-n2](https://github.com/user-attachments/assets/5dbe7711-7fcf-4618-a2da-48a23f911e3b)

### Larger Pattern Size (N=3)
Increasing \(N\) to 3 produces an image with more recognizable elements of the flowers:  
![output-n3](https://github.com/user-attachments/assets/004f54cc-49cb-4f94-b733-9d72f87bc289)

### Using Edge Locks
By enabling bottom locks, we ensure the “ground” always appears at the bottom:  
![output-locks](https://github.com/user-attachments/assets/67a6afed-dc7b-442e-8680-a046fb142c83)

### Using Reflections
If the input flowers lean right, enabling **Reflections** can fix that and make them lean both ways:  
![output-reflections](https://github.com/user-attachments/assets/a0d11910-960a-4290-90d0-019e6a2f874f)

### Using Rotations
With **Rotations** turned on, you get even more variety. For instance, here is a 50×50 result:  
![output-rotations](https://github.com/user-attachments/assets/eb984618-befa-4fd8-86c6-b3f7434094fe)

---

## Screenshots
Here’s a quick look at the app’s GUI:  
![screenshot](https://github.com/user-attachments/assets/f6b89f1e-bd8c-4b33-911b-dee387789e9b)

---

## Limitations
- **Performance**: Generation can be **very slow** for \(N > 4\), especially if the input image is larger than about 100×100 and it is straight up unusable for anything more.
- **Generalization**: The existing feature set might not handle all specialized use cases without further tweaks.
Overall this is not the fastest nor the most optimized implementation of the algorithm, but with small bitmaps and small color pallette it might just do the job and help to understand
the algorithm as there is little to no resources on this algorithm, especially the overlapping vesrion of it.
---

## Tooling
- **.NET 8**: The project targets .NET 8.
- **Avalonia UI**: Provides the cross-platform GUI framework.
- **C#**: The primary programming language.

---

## Running
There are builds available for windows and linux on the release page. Building from source should be straightforward, run `dotnet run` in the `WaveFunctionCollapse.Avalonia.Desktop` project.
