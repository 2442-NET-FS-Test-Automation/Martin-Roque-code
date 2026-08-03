import { BookCard } from "../../src/components/BookCard";
import type { InventoryItem } from "../../src/types";
import { MemoryRouter } from "react-router-dom";

describe("BookCard (component)", () => {
    const item: InventoryItem = { sku:"BK-001", name: "Clean Code", currentStock: 5};

    it("renders name, sku and stock", () => {
        cy.mount(
            <MemoryRouter>
                <BookCard item={item}/>
            </MemoryRouter>
        );

        cy.contains("h3", "Clean Code");
        cy.contains("dd", "BK-001");
        cy.contains("dd", "5");
    });

    it("marks a zero-stock item with the out class", () => {
        cy.mount(
            <MemoryRouter>
                <BookCard item={{ sku:"BK-001", name: "Clean Code", currentStock: 0}}/>
            </MemoryRouter>
        );

        cy.get("dd.out").should("have.text", 0);
    });
});