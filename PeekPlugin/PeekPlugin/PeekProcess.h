//
//  PeekProcess.h
//  PeekPlugin
//
//  Created by kenny on 16/06/2013.
//  Copyright (c) 2013 Disney Research. All rights reserved.
//

#ifndef PeekPlugin_PeekProcess_h
#define PeekPlugin_PeekProcess_h

void BGRAtoGray(uint8_t* __restrict dest, uint8_t* __restrict src, int numPixels);
int FindSpheres(uint8_t* __restrict src, int width, int height);

#endif
