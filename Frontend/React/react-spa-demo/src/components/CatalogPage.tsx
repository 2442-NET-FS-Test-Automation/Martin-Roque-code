import { useEffect, useState } from "react";
import { BookCard } from "./BookCard";
import { type InventoryItem, type FetchState, SortDirection } from "../types";
import { getInventory } from "../api/inventory";
import { SearchBar } from "./SearchBar";

export function CatalogPage() {
    //We are going to use a hook called useState to store information between component re-renders
    //Hooks are functions that allow you to do certain things based on the component lifecycle
    //Hook rules:
    //1.- Only call on hooks from the top level
    //2.- Only call Hooks from react functions

    //const [items] = useState(catalog);

    //const [compact, setCompact] = useState(false);
    const [items, setItems] = useState<InventoryItem[]>([]);
    const [fstate, setFState] = useState<FetchState>("idle");

    //Search + sort state - lifted from SearchBar and shared with children as needed.
    const [userQuery, setUserQuery] = useState("");
    const [dir, setDir] = useState<SortDirection>(SortDirection.Ascending);

    useEffect(() => {
        let active = true;

        setFState("loading");

        getInventory()
            .then((data) => {
                if(!active) return;
                setItems(data);
                setFState("loaded");
            })
            .catch(() => {
                if (active) setFState("failed");
            });
            //useEffect needs  cleanup function to be returned
            return () => {
                active = false;
            }
    }, []);

    // We don't want to filter the list in place
    const visibleBooks = [...items]
        .filter((i) => i.name.toLowerCase().includes(userQuery.toLowerCase()))
        .sort((a, b) => 
            dir === SortDirection.Ascending
                ? a.name.localeCompare(b.name)
                : b.name.localeCompare(a.name)
        );

    if(fstate === "idle" || fstate === "loading") return <p>Loading catalog...</p>

    if(fstate === "failed") 
        return <p>Could not reach API. Is it running? Check CORS</p>

    return (
        <section>
            <div className="toolbar">
                <h2>Catalog</h2>
                <SearchBar value={userQuery} onChange={setUserQuery}/>
                <button 
                    type="button"
                    onClick={() => 
                        setDir((d) =>
                            d === SortDirection.Ascending
                                ? SortDirection.Descending
                                : SortDirection.Ascending 
                        )
                    }    
                >
                    Sort {dir === SortDirection.Ascending ? "Z-A" : "A-Z"}
                </button>
            </div>

            {visibleBooks.length === 0 ? (
                <p>No books match {userQuery}</p>
            ) : (
                <div className="cards">
                    {visibleBooks.map((item) => (
                        <BookCard key={item.sku} item={item}/>
                    ))}
                </div>
            )}
        </section>
    );

    // return(
    //     <>
    //         <div className="tooldbar">
    //             <h2>Catalog</h2>
    //             <button type="button" onClick={() => setCompact((c) => !c)}>
    //                 {compact ? "Show detail" : "Compact view"}
    //             </button>
    //         </div>

    //         <div className="cards">
    //             {
    //                 items.map((item) => (
    //                     <BookCard key={item.sku} item={item} compact={compact}/>
    //                 ))
    //             }
    //         </div>

    //     </>
    // )
}