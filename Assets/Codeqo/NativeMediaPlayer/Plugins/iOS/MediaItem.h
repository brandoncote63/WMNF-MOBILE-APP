//
//  MediaItem.h
//  Receive MediaItem Information from Unity
//
//  Created by Yohan Song 5/10/22.
//  Copyright © 2022 Codeqo. All rights reserved.
//

#import <AVFoundation/AVFoundation.h>
#import <MediaPlayer/MPMediaItem.h>
#import <MediaPlayer/MPNowPlayingInfoCenter.h>
#import "PlayerPrefs.h"

typedef NS_ENUM(NSInteger, ArtworkType) {
    RETRIEVE_ARTWORK,
    ADD_CUSTOM_ARTWORK,
    ADD_CUSTOM_ARTWORK_WHEN_ARTWORK_IS_UNAVAILABLE
};

typedef NS_ENUM(NSInteger, AddIndexType) {
    ADD_INDEX_NONE,
    ADD_INDEX_PREFIX,
    ADD_INDEX_SUFFIX 
};

@interface MediaItem : NSObject
{
    bool      customMetadata;
    bool      prepared;
    int       addIndexType;
    NSString* defaultAlbumTitle;
    NSString* defaultAlbumArtist;
    NSString* defaultTitle;
    NSString* defaultArtist;
}

- (id) initWithJson: (int)_index count:(int)_count pathType:(int)_pathType json:(NSString*)_json;
- (void) retrieveMediaMetadata;
- (bool) isPrepared;

@property(nonatomic) int MediaItemIndex;
@property(nonatomic) int MediaLocation;
@property(nonatomic) int Count; // total # of tracks
@property(nonatomic, retain) NSURL* MediaUri;
@property(nonatomic, retain) NSString *AlbumTitle;
@property(nonatomic, retain) NSString *AlbumArtist;
@property(nonatomic, retain) NSString *Title;
@property(nonatomic, retain) NSString *Artist;
@property(nonatomic, retain) NSString *Genre;
@property(nonatomic, retain) NSString *ReleaseDate;
@property(nonatomic, retain) NSData   *ArtData;
@property(nonatomic, retain) MPMediaItemArtwork* Artwork;
@end
