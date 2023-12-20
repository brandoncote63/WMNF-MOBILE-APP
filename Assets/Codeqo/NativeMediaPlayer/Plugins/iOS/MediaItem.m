//
//  MediaItem.m
//  Receive MediaItem Information from Unity
//
//  Created by Yohan Song 5/10/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import "MediaItem.h"
#import "JsonUtility.h"
#import "SharedVariables.h"
#import "NativeMediaPlayer.h"

@interface MediaItem()
- (void) applyDefaultMediaMetadata;
- (void) resetDefaultMediaMetadata;
- (void) fillInEmptyMediaMetadata;
- (NSString*) addIndex: (NSString*)_data;
- (NSString*) getDefaultAlbumTitle;
- (NSString*) getDefaultAlbumArtist;
- (NSString*) getDefaultTitle;
- (NSString*) getDefaultArtist;
@end

@implementation MediaItem
@synthesize AlbumTitle;
@synthesize AlbumArtist;
@synthesize Title;
@synthesize Artist;
@synthesize Genre;
@synthesize ReleaseDate;
@synthesize ArtData;


- (id) initWithJson: (int)_index count:(int)_count pathType:(int)_pathType json:(NSString*)_json {
	if(self = [super init]) {
        prepared = false;
        [self resetDefaultMediaMetadata];
        self.MediaItemIndex = _index;
        self.Count = _count;
        self.MediaLocation = _pathType;
        
        NSData* jsonData = [_json dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error = nil;
        NSDictionary *mediaJson = [NSJSONSerialization
                                      JSONObjectWithData:jsonData
                                      options:0
                                      error:&error];
        if (mediaJson != nil) {
            customMetadata = [[mediaJson objectForKey:@"metadataType"] isEqual: @1];
            NSString* path = [mediaJson objectForKey:@"uri"];
        
            if (self.MediaLocation == STREAMING_ASSET) {
                NSString* fileName = path.stringByDeletingPathExtension;
                NSString* fileExt = path.pathExtension;
                path = [[NSBundle mainBundle] pathForResource:fileName ofType:fileExt inDirectory:@"Data/Raw"];
                self.MediaUri = [NSURL fileURLWithPath:path];
                NSLog(@"%@ : StreamingAssets", self.MediaUri);
            } else {
                self.MediaUri = [NSURL URLWithString:path];
                NSLog(@"%@ : Url", self.MediaUri);
            }
        
           
            if (customMetadata) {
                self.AlbumTitle = [mediaJson objectForKey:@"albumTitle"];
                self.AlbumArtist = [mediaJson objectForKey:@"albumArtist"];
                self.Title = [mediaJson objectForKey:@"title"];
                self.Artist = [mediaJson objectForKey:@"artist"];
                self.Genre = [mediaJson objectForKey:@"genre"];
                self.ReleaseDate = [mediaJson objectForKey:@"date"];
                NSString *artData =[mediaJson objectForKey:@"artworkDataBase64"];
                if (artData != (NSString*)[NSNull null]) {
                    self.ArtData = [[NSData alloc] initWithBase64EncodedString:artData
                                                                       options:NSDataBase64DecodingIgnoreUnknownCharacters];
                    self.Artwork = [self getCustomArtwork];
                }                
                
                [self fillInEmptyMediaMetadata];
                NSLog(@"%d.%@ : Custom Media Metadata Loaded", self.MediaItemIndex, self.Title);
            }
            else [self retrieveMediaMetadata];
        }
	}
	return self;
}

- (bool) isPrepared {
    NSLog(@"%@", prepared ? @"Item Prepared" : @"Item Not Prepared");
    return prepared;
}

- (void) retrieveMediaMetadata {
    /* Creating AVAasset to Retrieve Media Metadata */
    AVAsset *asset = [AVAsset assetWithURL:self.MediaUri];

    [asset loadValuesAsynchronouslyForKeys:@[@"commonMetadata"] completionHandler:^{
        NSError *error = nil;
        AVKeyValueStatus status = [asset statusOfValueForKey:@"commonMetadata" error:&error];

        switch (status) {
            case AVKeyValueStatusLoaded:
            {
                NSArray *artworks = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeyArtwork keySpace:AVMetadataKeySpaceCommon];
                NSArray *albumTitles = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeyAlbumName keySpace:AVMetadataKeySpaceCommon];
                NSArray *albumArtists = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeyAuthor keySpace:AVMetadataKeySpaceCommon];
                NSArray *titles = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeyTitle keySpace:AVMetadataKeySpaceCommon];
                NSArray *artists = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeyArtist keySpace:AVMetadataKeySpaceCommon];
                NSArray *genres = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeySubject keySpace:AVMetadataKeySpaceCommon];
                NSArray *releaseDates = [AVMetadataItem metadataItemsFromArray:asset.commonMetadata withKey:AVMetadataCommonKeyCreationDate keySpace:AVMetadataKeySpaceCommon];
                
                for (AVMetadataItem* item in artworks) {
                    if ([item.keySpace isEqualToString:AVMetadataKeySpaceID3]) {
                        if (TARGET_OS_IPHONE && NSFoundationVersionNumber > NSFoundationVersionNumber_iOS_7_1) {
                            NSData *newImage = [item.value copyWithZone:nil];
                            self.ArtData = newImage;
                            self.Artwork = [self dataToMPMediaItemArtwork:newImage];
                        } else {
                            NSDictionary *dict = [item.value copyWithZone:nil];
                            if ([dict objectForKey:@"data"]) {
                                self.ArtData = [dict objectForKey:@"data"];
                                self.Artwork = [self dataToMPMediaItemArtwork:[dict objectForKey:@"data"]];
                            }
                        }
                    } else if ([item.keySpace isEqualToString:AVMetadataKeySpaceiTunes]) {
                        self.ArtData = [item.value copyWithZone:nil];
                        self.Artwork = [self dataToMPMediaItemArtwork:[item.value copyWithZone:nil]];
                    }
                }

                self.AlbumTitle = [((AVMetadataItem*)[albumTitles firstObject]).value copyWithZone:nil];
                self.AlbumArtist = [((AVMetadataItem*)[albumArtists firstObject]).value copyWithZone:nil];
                self.Title = [((AVMetadataItem*)[titles firstObject]).value copyWithZone:nil];
                self.Artist = [((AVMetadataItem*)[artists firstObject]).value copyWithZone:nil];
                self.Genre = [((AVMetadataItem*)[genres firstObject]).value copyWithZone:nil];
                self.ReleaseDate = [((AVMetadataItem*)[releaseDates firstObject]).value copyWithZone:nil];

                NSLog(@"%d.%@ : Media Metadata Retrieved", self.MediaItemIndex, self.Title);
                [self fillInEmptyMediaMetadata];
                break;
            }
            case AVKeyValueStatusFailed:
                NSLog(@"Retrieving media metadata failed : Loading AVAsset failed");
                [self applyDefaultMediaMetadata];
                break;
            case AVKeyValueStatusCancelled:
                NSLog(@"Retrieving media metadata failed : Loading AVAsset cancelled");
                [self applyDefaultMediaMetadata];
                break;
            default:
                NSLog(@"Retrieving media metadata failed : Unknown");
                [self applyDefaultMediaMetadata];
                break;
        }
    }];
}

- (void) fillInEmptyMediaMetadata {
    if (self.AlbumTitle == nil)
        self.AlbumTitle = [self getDefaultAlbumTitle];
    if (self.AlbumArtist == nil)
        self.AlbumArtist = [self getDefaultAlbumArtist];
    if (self.Title == nil)
        self.Title = [self getDefaultTitle];
    if (self.Artist == nil)
        self.Artist = [self getDefaultArtist];
    if (self.Genre == nil)
        self.Genre = @"Unknown";
    if (self.ReleaseDate == nil)
        self.ReleaseDate = @"Unknown";
    
    prepared = true;
}

- (void) applyDefaultMediaMetadata {
    self.AlbumTitle = [self getDefaultAlbumTitle];
    self.AlbumArtist = [self getDefaultAlbumArtist];
    self.Title = [self getDefaultTitle];
    self.Artist = [self getDefaultArtist];
    self.Genre = @"Unknown";
    self.ReleaseDate = @"Unknown";
}

- (MPMediaItemArtwork*) dataToMPMediaItemArtwork: (NSData*)_data {
    UIImage *tempImg = [UIImage imageWithData:_data];
    return [[MPMediaItemArtwork alloc] initWithBoundsSize:CGSizeMake(600, 600) requestHandler: ^UIImage* _Nonnull(CGSize size) {
        return tempImg;
    }];
}

- (void) resetDefaultMediaMetadata {
    addIndexType = [[PlayerPrefs sharedObject] getInt:@"DEFAULT_ADD_INDEX_TYPE"];
    defaultAlbumTitle = [self addIndex:[[PlayerPrefs sharedObject] getString:@"DEFAULT_ALBUM"]];
    defaultAlbumArtist = [self addIndex:[[PlayerPrefs sharedObject] getString:@"DEFAULT_ALBUM_ARTIST"]];
    defaultTitle = [self addIndex:[[PlayerPrefs sharedObject] getString:@"DEFAULT_TITLE"]];
    defaultArtist = [self addIndex:[[PlayerPrefs sharedObject] getString:@"DEFAULT_ARTIST"]];
}

- (NSString*) addIndex: (NSString*)_data {
    if (addIndexType == ADD_INDEX_PREFIX) {
        return [NSString stringWithFormat:@"%d%@", (self.MediaItemIndex + 1), _data];
    } else if (addIndexType == ADD_INDEX_SUFFIX) {
        return [NSString stringWithFormat:@"%@%d", _data, (self.MediaItemIndex + 1)];
    } else {
        return _data;
    }
}

- (NSString*) getDefaultAlbumTitle {
    return defaultAlbumTitle;
}

- (NSString*) getDefaultAlbumArtist {
    return defaultAlbumArtist;
}

- (NSString*) getDefaultTitle {
    return defaultTitle;
}

- (NSString*) getDefaultArtist {
    return defaultArtist;
}

- (MPMediaItemArtwork*) getCustomArtwork {
    if (self.ArtData) {
        return [self dataToMPMediaItemArtwork:self.ArtData];
    }
    return nil;
}

@end
