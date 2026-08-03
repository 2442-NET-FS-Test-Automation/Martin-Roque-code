import { CatalogPage } from "../pages/CatalogPage";

describe("catalog via a page object", () => {
    const catalog = new CatalogPage();

    it("filters through the page object", () => {
        catalog.visit().search("Clean");
        catalog.cards().should("have.length", 1);
        catalog.firstTitle().should("contain.text", "Clean Code");
    });

    it("sorts through the page object", () => {
        catalog.visit().toggleSort();
        catalog.firstTitle().should("contain.text", "The Pragmatic Programmer");
    });
});