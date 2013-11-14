#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>

#import <OpenGLES/ES2/gl.h>
#import <OpenGLES/ES2/glext.h>

#define ASYNC_QUEUE 0

#if ASYNC_QUEUE
void runAsynchronouslyOnVideoProcessingQueue(void (^block)(void));
#endif

@interface VideoCameraDelegate : NSObject <AVCaptureVideoDataOutputSampleBufferDelegate>
{
	AVCaptureSession *session;
	
	GLuint _textureId;
	int _width;
	int _height;
    
	UIInterfaceOrientation _orientation;
}
@property (nonatomic, assign) GLuint textureId;
@property (nonatomic, assign) int width;
@property (nonatomic, assign) int height;

/** Process a video sample
 @param sampleBuffer Buffer to process
 */
- (void)captureOutput:(AVCaptureOutput *)captureOutput didOutputSampleBuffer:(CMSampleBufferRef)sampleBuffer fromConnection:(AVCaptureConnection *)connection;
- (void)didDropSampleBuffer:(CMSampleBufferRef)sampleBuffer;

+ (VideoCameraDelegate*)sharedManager;
+ (BOOL)isCaptureAvailable;
+ (BOOL)isCameraAdjusting;
+ (NSArray*)availableDevices;

- (void)startCameraCaptureWithDevice:(NSString*)deviceId;
- (void)stopCameraCapture;
- (void)setExposureMode:(AVCaptureExposureMode)exposureMode;
- (void)setFocusMode:(AVCaptureFocusMode)focusMode;

@end;
