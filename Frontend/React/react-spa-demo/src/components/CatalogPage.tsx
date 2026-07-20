import { useState } from "react";
import { BookCard } from "./BookCard";
import { catalog } from "../data/catalog";

export function CatalogPage() {
    //We are going to use a hook called useState to store information between component re-renders
    //Hooks are functions that allow you to do certain things based on the component lifecycle
    //Hook rules:
    //1.- Only call on hooks from the top level
    //2.- Only call Hooks from react functions

    const [items] = useState(catalog);

    const [compact, setCompact] = useState(false);

    return(
        <>
            <div className="tooldbar">
                <h2>Catalog</h2>
                <button type="button" onClick={() => setCompact((c) => !c)}>
                    {compact ? "Show detail" : "Compact view"}
                </button>
            </div>

            <div className="cards">
                {
                    items.map((item) => (
                        <BookCard key={item.sku} item={item} compact={compact}/>
                    ))
                }
            </div>

        </>
    )
}