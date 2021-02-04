import { useCallback, useEffect, useRef, useState } from "react";
import { DropMeter, PluginMessage } from "./DropMeter";

function useStateWithPromise<T>(initialState:T) {
    const [state, setState] = useState<T>(initialState);
    const resolverRef = useRef<any>(null);
  
    useEffect(() => {
      if (resolverRef.current) {
        resolverRef.current(state);
        resolverRef.current = null;
      }
      /**
       * Since a state update could be triggered with the exact same state again,
       * it's not enough to specify state as the only dependency of this useEffect.
       * That's why resolverRef.current is also a dependency, because it will guarantee,
       * that handleSetState was called in previous render
       */
    }, [resolverRef.current, state]);
    const handleSetState = useCallback((stateAction) => {
        setState(stateAction);
        return new Promise(resolve => {
          resolverRef.current = resolve;
        });
      }, [setState])
    
      return [state, handleSetState] as [T, (stateAction: any)=>Promise<any>];
    }

//let registeredMessages = new Map<string, 
export function usePluginDataAsState<RET, MES>(pluginId: string, messageId: MES){
    const [state, setState] = useState<RET|null>(null)
    useEffect(() => {
        (async ()=>{
        (await DropMeter()).registerCallback(pluginId, (data: PluginMessage<MES, RET>)=> {
            if(data.messageID == messageId) setState(data.data)
        })
    })()
    }, [])
    return state;
}

export function usePluginAsState<T extends PluginMessage>(pluginId: string, messageId: T["messageID"]){
    const [state, setState] = useState<T["data"]|null>(null)
    useEffect(() => {
        (async ()=>{
        (await DropMeter()).registerCallback(pluginId, (data)=> {
            if(data.messageID == messageId) setState(data.data)
        })
    })()
    }, [])
    return state;
} 

export let usePluginCombinedState = (pluginId: string) => {
    
    const [state, setState] = useState<any>({TITLE: "Wait..."})
    useEffect(() => {
        (async ()=>{
        (await DropMeter()).registerCallback(pluginId, async (data: PluginMessage<string, any>)=> {
            
            
            
            setState((oldData: any) => {
                let newdata = {...oldData }
                newdata[data.messageID] = data.data
                return newdata;
            });
        })
    })()
    }, [])
    return state;
}