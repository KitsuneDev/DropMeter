export interface IDropMeter {
    registerCallback<T, T2>(pluginId: string, callback: (data: PluginMessage<T,T2>)=>void) : void;
    sendToPlugin(): void;
}


export interface PluginMessage<MESG = string, DATA = any> {
    messageID: MESG;
    data: DATA
}

interface MockEncapsulatedMessage {
    pluginId: string,
    message: PluginMessage<any, any>
}

export class MockDropMeter implements IDropMeter {
    mockSocket: WebSocket;
    listeners: {
        plugin: string,
        cb: (data: PluginMessage<any, any>) => void
    }[] = []
    constructor() {
        this.mockSocket = new WebSocket("ws://localhost:9007/pluginchannel");
        this.mockSocket.onopen = (ev) => {
            console.log("Connected to Local Mock Server")
            //this.mockSocket.send("core:id")
            
        }
        this.mockSocket.onmessage = (ev) => {
            let msgParts = JSON.parse(ev.data) as MockEncapsulatedMessage;
            //console.log("Got from mock: ", msgParts)
            let targets = this.listeners.filter(x=>x.plugin == msgParts.pluginId)
            for(let target of targets){
                target.cb(msgParts.message)
            }
        }

    }
    registerCallback<T, T2>(pluginId: string, callback: (data: PluginMessage<T, T2>) => void): void {
        this.listeners.push({
            plugin: pluginId,
            cb: callback
        })
    }
    sendToPlugin() {};
    
}

export const isMock = ((window as any).CefSharp == null)
export var mockInstance: MockDropMeter|null = null;
async function GetDropMeter(){
    if(isMock){
        if(mockInstance == null) mockInstance = new MockDropMeter();
        return mockInstance;

    }
else if((window as any).DropMeter == null){
    await (window as any).CefSharp.BindObjectAsync("DropMeter")
}
return (window as any).DropMeter as IDropMeter;
}
//declare var DropMeter: IDropMeter;

export const DropMeter = GetDropMeter;

