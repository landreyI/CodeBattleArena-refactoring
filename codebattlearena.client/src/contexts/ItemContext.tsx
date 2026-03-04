import { ReactNode } from "react";
import { createContext, useContext } from "react";
import { Item } from "../models/dbModels";

export const ItemContext = createContext<Item | null>(null);
export const useItem = () => useContext(ItemContext);

export const ItemProvider = ({ item, children }: { item: Item | null, children: ReactNode }) => (
    <ItemContext.Provider value={item}>
        {children}
    </ItemContext.Provider>
);