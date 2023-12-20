//
//  JsonUtility.m
//  UnityFramework
//
//  Created by Munchkin Productions on 2022/08/20.
//

#import "JsonUtility.h"

@interface JsonUtility()
-(NSString*) getPath:(NSString*)_key;
@end

@implementation JsonUtility // public methods

+ (JsonUtility*) sharedObject {
    static JsonUtility *sharedObject = nil;
    if (!sharedObject) {
        sharedObject = [[JsonUtility alloc] init];
    }
    return sharedObject;
}

- (NSString*) getPath: (NSString*)_key {
    NSString *dir = [NSMutableString stringWithFormat:@"%s/%@.json", UnityDocumentsDir(), _key];
    return dir;
}

- (NSDictionary *) loadNSDictionaryFromJson: (NSString*)_key {
    NSError *error = nil;
    NSLog(@"%@", [self getPath:_key]);
    NSString* jsonString = [NSString stringWithContentsOfFile:[self getPath:_key]
                                                     encoding:NSUTF8StringEncoding
                                                        error:&error];
    NSData *data = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    return [NSJSONSerialization JSONObjectWithData:data
                                           options:kNilOptions
                                             error:&error];
}

- (void) saveNSDictionaryToJson: (NSString*)_key dictionary:(NSDictionary *)_dict {
    /*
    NSMutableDictionary *dicJSON = [[NSMutableDictionary alloc] init];
    dicJSON[@"name"] = @"babbab";
    dicJSON[@"age"] = @"25";
    dicJSON[@"gender"] = @"female";*/
    
    NSError *error = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:_dict
                                                       options:NSJSONWritingPrettyPrinted
                                                         error:&error];
    [jsonData writeToFile:[self getPath:_key] atomically:YES];
}

@end

