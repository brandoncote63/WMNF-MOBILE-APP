//
//  PlayerPrefs.h (Singleton)
//  PlayerPrefs Emulator for Unity iOS Plugins 
//
//  Created by Yohan Song 5/10/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

@interface PlayerPrefs : NSObject

+ (PlayerPrefs*) sharedObject;

- (NSString*) getString:(NSString*)_key;
- (NSString*) getString:(NSString*)_key id:(int)_id;
- (void) setString:(NSString*)_key value:(NSString*)_value;

- (int) getInt:(NSString*)_key;
- (int) getInt:(NSString*)_key id:(int)_id;
- (void) setInt:(NSString*)_key value:(int)_value;

- (float) getFloat:(NSString*)_key;
- (float) getFloat:(NSString*)_key id:(int)_id;
- (void) setFloat:(NSString*)_key value:(float)_value;

- (void) deleteKey:(NSString*)_key;

@end
