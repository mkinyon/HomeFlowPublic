# ImageFile Update Functionality

This document describes how to use the ImageFile update functionality in HomeFlow.

## Overview

The ImageFile update functionality allows you to replace the content of an existing image file while keeping the same ID and database record. This is useful when you want to update an image without breaking any existing references to it.

## Usage

### Service Method

```csharp
// Inject the service
[Inject] IImageFileService ImageFileService { get; set; } = default!;

// Update an existing image file
var updatedImage = await ImageFileService.UpdateAsync(
    existingImageId, 
    new ImageFileRequest 
    { 
        Data = newImageBytes,
        Width = 800,
        Height = 600,
        Folder = "recipes"
    }
);
```

### Direct Command Usage

```csharp
// Inject the sender
[Inject] ISender Sender { get; set; } = default!;

// Update an existing image file
var updatedImage = await Sender.Send(new UpdateImageFileCommand(
    existingImageId,
    new ImageFileRequest 
    { 
        Data = newImageBytes,
        Width = 800,
        Height = 600,
        Folder = "recipes"
    }
));
```

## Features

- **Preserves ID**: The existing image file ID is maintained
- **File System Update**: The actual file in the file system is replaced
- **Database Consistency**: The database record is updated if the folder changes
- **Image Processing**: The new image is processed and resized according to the specified dimensions
- **Validation**: Input validation ensures data integrity

## Error Handling

The update operation will throw:
- `NotFoundException` if the image file ID doesn't exist
- `ArgumentException` if the image dimensions are invalid
- Validation errors if the request data is invalid

## Example Use Cases

1. **Profile Picture Updates**: Update a user's profile picture without changing the URL
2. **Recipe Image Updates**: Replace a recipe image with a better quality version
3. **Content Management**: Update images in a CMS without breaking existing links
4. **Image Optimization**: Replace large images with optimized versions
