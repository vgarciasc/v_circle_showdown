// SpriteGenerator - a simple java program for creating vertical sprites from a folder of PNG images.
//
// The software is released free of any copyright of license restrictions.
// Modified by Mark Young (http://www.zarzax.com) April 2011
// originally by Peter Moberg (http://sourcecodebean.com/archives/a-simple-image-sprite-generator-in-java/1065) April 2011
import javax.imageio.ImageIO;
import java.awt.*;
import java.awt.image.BufferedImage;
import java.io.*;
import java.util.*;
 
public class SpriteGenerator {
 
    public static void main(String[] args) throws IOException {

        if (args.length != 2)
        {
           System.out.print("Usage: SpriteGenerator {name of pattern} {margin between images in px} {N such as the mosaic is NxN} {output file}\n");
           System.out.print("Note: The max height should only be around 32,767px due to Microsoft GDI using a 16bit signed integer to store dimensions\n");
           System.out.print("going beyond this dimension is possible with this tool but the generated sprite image will not work correctly with\n");
           System.out.print("most browsers.\n\n");
           return;
        }
 
        String patternName = args[0];
        Integer dimension = Integer.parseInt(args[1]);
 
        File patternFile = new File(System.getProperty("user.dir") + "\\" + patternName + ".png");

        // Read images
        ArrayList<BufferedImage> imageList = new ArrayList<BufferedImage>();
        BufferedImage bufferedImage = ImageIO.read(patternFile);

        // Find max width and total height
        int maxWidth = bufferedImage.getWidth() * dimension;
        int maxHeight = bufferedImage.getHeight() * dimension;
 
        System.out.format("Number of images: %s, max height: %spx, width: %spx%n",
                                      imageList.size(), maxHeight, maxWidth);
 
 
        // Create the actual sprite
        BufferedImage sprite = new BufferedImage(maxWidth, maxHeight,
                                                      BufferedImage.TYPE_INT_ARGB);
 
        int currentY = 0;
        Graphics g = sprite.getGraphics();
        for (int i = 0; i < dimension; i++) {
            for (int j = 0; j < dimension; j++) {
                g.drawImage(bufferedImage, i * bufferedImage.getHeight(), j * bufferedImage.getWidth(), null);
            }
        }
 
        System.out.println("Success.");
        ImageIO.write(sprite, "png", new File(System.getProperty("user.dir") + "\\" + patternName + "_mosaic.png"));
    }
}