interface DropMeter {
    registerCallback: (pluginId: string, messageId: string, callback: (data: any)=>void) => void;
    sendToPlugin: ()=>void;
}


if((window as any).DropMeter == null){
    (window as any).CefSharp.CefSharp.BindObjectAsync("DropMeter")
}

declare var DropMeter: DropMeter;

export {DropMeter};

