//
//  JsonUtility.h
//  Unity-iPhone
//
//  Created by Munchkin Productions on 2022/08/20.
//

#import <Unity/UnityInterface.h>

@interface JsonUtility : NSObject

+ (JsonUtility*) sharedObject;
- (NSDictionary*) loadNSDictionaryFromJson:(NSString*)_key;
- (void) saveNSDictionaryToJson:(NSString*)_key dictionary:(NSDictionary*)_dict;

@end

