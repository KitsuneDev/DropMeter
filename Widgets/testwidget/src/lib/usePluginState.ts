import { useEffect, useState } from "react";
import { DropMeter } from "./DropMeter";

export function usePluginAsState(pluginId: string, messageId: string){
    const [state, setState] = useState(null)
    useEffect(() => {
        DropMeter.registerCallback(pluginId, messageId, (data)=> {
            setState(data)
        })
    }, [])
    return state;
}