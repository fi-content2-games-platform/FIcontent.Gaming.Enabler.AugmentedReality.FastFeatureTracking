//
//  PeekProcess.cpp
//  PeekPlugin
//
//  Created by kenny on 16/06/2013.
//  Copyright (c) 2013 Disney Research. All rights reserved.
//

#define USE_OPENCV 0

#include <arm_neon.h>

#if USE_OPENCV
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#endif

#include "PeekProcess.h"

void BGRAtoGray(uint8_t* __restrict dest, uint8_t* __restrict src, int numPixels)
{
    int n=numPixels;
   /* int i;
    int r,g,b;
    int y;
    
    for (i=0; i < n; i++)
    {
        b = *src++; // load blue
        g = *src++; // load green
        r = *src++; // load red
        src++; // skip aplha
        
        // build weighted average:
        y = (r*77)+(g*151)+(b*28);
        
        // undo the scale by 256 and write to memory:
        *dest++ = (y>>8);
    }*/
    int i;
    uint8x8_t rfac = vdup_n_u8 (77);
    uint8x8_t gfac = vdup_n_u8 (151);
    uint8x8_t bfac = vdup_n_u8 (28);
    n>>=3;

    // Convert per eight pixels
    for (i=0; i<n; ++i)
    {
        uint16x8_t  temp;
        uint8x8x4_t rgb  = vld4_u8 (src);
        uint8x8_t result;

        temp = vmull_u8 (rgb.val[0],      bfac);
        temp = vmlal_u8 (temp,rgb.val[1], gfac);
        temp = vmlal_u8 (temp,rgb.val[2], rfac);

        result = vshrn_n_u16 (temp, 8);
        vst1_u8 (dest, result);
        src  += 8*4;
        dest += 8;
    }
  /* asm volatile(
     "# build the three constants: \n"
     "mov         r4, #28          \n" // Blue channel multiplier
     "mov         r5, #151         \n" // Green channel multiplier
     "mov         r6, #77          \n" // Red channel multiplier
     "vdup.8      d4, r4           \n"
     "vdup.8      d5, r5           \n"
     "vdup.8      d6, r6           \n"
     ".loop:                       \n"
     "# load 8 pixels:             \n"
     "vld4.8      {d0-d3}, [%2]!   \n"
     "# do the weight average:     \n"
     "vmull.u8    q7, d0, d4       \n"
     "vmlal.u8    q7, d1, d5       \n"
     "vmlal.u8    q7, d2, d6       \n"
     "# shift and store:           \n"
     "vshrn.u16   d7, q7, #8       \n" // Divide q3 by 256 and store in the d7
     "vst1.8      {d7}, [%0]!      \n"
     "subs        %1, %1, #8       \n" // Decrement iteration count
     "bne         .loop            \n" // Repeat unil iteration count is not zero
     :
     : "r" (dest), "r" (n), "r" (src)
     : "memory", "r4", "r5", "r6", "q0", "q1", "q3", "q7"
     );*/
}

int FindSpheres(uint8_t* __restrict src, int width, int height)
{
#if USE_OPENCV
    cv::Mat src_gray(width, height, CV_8UC1, src, 0);

    // Do something with the raw pixels here
    /// Reduce the noise so we avoid false circle detection
    //cv::GaussianBlur( src_gray, src_gray, cv::Size(5, 5), 0, 0 );
    
    cv::vector<cv::Vec3f> circles;

    /// Apply the Hough Transform to find the circles
   // cv::HoughCircles( src_gray, circles, CV_HOUGH_GRADIENT, 1, src_gray.rows/8, 200, 100, 0, 0 );

    return circles.size();
#else
    return 0;
#endif
}
