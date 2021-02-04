
import { PluginMessage } from "./DropMeter";

export type WebNowPlayingState = 
{
STATE: 1 & 2
TITLE: string
ARTIST: string
ALBUM: string
POSITION: string
DURATION: string
VOLUME: number
RATING: 5 & 0
REPEAT: 0 & 1 & 2
SHUFFLE: 0 & 1
COVER: string

}

export type WebNowPlayingMessages = 
PluginMessage<"STATE", 1 | 2> |
PluginMessage<"TITLE", string> |
PluginMessage<"ARTIST", string> |
PluginMessage<"ALBUM", string> |
PluginMessage<"POSITION", string> |
PluginMessage<"DURATION", string> |
PluginMessage<"VOLUME", number> |
PluginMessage<"RATING", 5 | 0> |
PluginMessage<"REPEAT", 0 | 1 | 2> |
PluginMessage<"SHUFFLE", 0 | 1> |
PluginMessage<"COVER", string>
