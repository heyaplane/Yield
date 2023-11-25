public static class ColorHelper
{
    public static uint GetColorARGB(float r, float g, float b, float a)
    {
        uint alpha = (uint) a * 255;
        uint red = (uint) r * 255;
        uint green = (uint) g * 255;
        uint blue = (uint) b * 255;
        return (alpha << 24) | (red << 16) | (green << 8) | blue;
    }
}
