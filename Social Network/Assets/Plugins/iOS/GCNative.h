#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

@interface GCNative : NSObject

-(void)ReportAchievement : (NSString*)achievementID : (float) progress;

@end
