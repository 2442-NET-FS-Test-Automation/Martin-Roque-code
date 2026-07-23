import { useState, useRef } from "react";
import type { SubmitEvent } from "react";
import { createBook, deleteBook } from "../api/inventory";
import { useAuth } from "../auth/useAuth";


export function AdminPage() {
    const { user } = useAuth();
    const [sku, setSku] = useState("");
    const [name, setName] = useState("");
    const [price, setPrice] = useState(0);
    const [stock, setStock] = useState(0);
    const [message, setMessage] = useState<string | null>(null);

    //Uncontrolled input/form
    const quickRef = useRef<HTMLInputElement>(null);
    function copyFromQuickFind() {
        if(quickRef.current) setSku(quickRef.current.value);
    }

    async function onCreate(e: SubmitEvent<HTMLFormElement>) {
        e.preventDefault();
        setMessage(null);

        try{
            const created = await createBook({
                sku,
                name,
                price,
                currentStock: stock
            });

            setMessage(`Created ${created.sku} - ${created.name}`);
            setSku("");
            setName("");
            setPrice(0);
            setStock(0);
        } catch {
            setMessage("Create failed - check fields, you may lack admin role.");
        }
    }

    async function onDelete() {
        if (!sku) return;

        setMessage(null);
        try{
            await deleteBook(sku);
            setMessage(`Deleted ${sku}`)
            setSku("");
        }catch{
            setMessage(`Deleted failed for ${sku}`)
        }
    }

    return (
        <section>
            <h2>Admin - {user?.name}</h2>
            <form className="admin-form" onSubmit={onCreate}>
                <input type="text" placeholder="SKU" value={sku} 
                    onChange={(e) => setSku(e.target.value)}/>
                <input type="text" placeholder="Name" value={name} 
                    onChange={(e) => setName(e.target.value)}/>
                <input type="number" value={price} 
                    onChange={(e) => setPrice(e.target.valueAsNumber)}/>
                <input type="number" value={stock} 
                    onChange={(e) => setStock(e.target.valueAsNumber)}/>
                <button type="submit">Create</button>
                <button type="button" onClick={onDelete}>
                    Deleted by sku
                </button>
            </form>

            <div className="quick-find">
                <input ref={quickRef} defaultValue="" placeholder="Quick SKU (Uncontrolled)"/>
                <button type="button" onClick={copyFromQuickFind}>
                    Copy into form
                </button>
            </div>

            {message && <p>{message}</p>}
        </section>
    );
}