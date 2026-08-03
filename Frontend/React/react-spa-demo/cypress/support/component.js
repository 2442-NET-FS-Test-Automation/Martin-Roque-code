import "./commands";
//import "@cypress/code-coverage/support";
import { mount } from "cypress/react";
import "../../src/App.css";
import "../../src/index.css"; 

Cypress.Commands.add("mount", mount);