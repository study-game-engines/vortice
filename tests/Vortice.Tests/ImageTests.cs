// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using NUnit.Framework;
using Alimer.Imaging;

namespace Alimer.Tests;

[TestFixture(TestOf = typeof(Image))]
public class ImageTests
{
    [TestCase]
    public void KTX1_Load()
    {
        string texturesFolder = Path.Combine(AppContext.BaseDirectory, "assets", "textures");
        Image image = Image.FromFile(Path.Combine(texturesFolder, "checkerboard_rgba.ktx"))!;
    }
}
