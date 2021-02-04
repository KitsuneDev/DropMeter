export interface IDropMeter {
    registerCallback<T, T2>(pluginId: string, callback: (data: PluginMessage<T,T2>)=>void) : void;
    sendToPlugin: ()=>void;
}


export interface PluginMessage<MESG = string, DATA = any> {
    messageID: MESG;
    data: DATA
}

async function GetDropMeter(){
if((window as any).DropMeter == null){
    await (window as any).CefSharp.BindObjectAsync("DropMeter")
}
return (window as any).DropMeter as IDropMeter;
}
//declare var DropMeter: IDropMeter;

export const DropMeter = GetDropMeter;

