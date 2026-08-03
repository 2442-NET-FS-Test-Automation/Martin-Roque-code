import { defineConfig} from "cypress";
import registerCodeCoverage from "@cypress/code-coverage/task";
import getCompareSnapshotsPlugin from "cypress-image-diff-js/plugin";

export default defineConfig({
    e2e: {
        baseUrl: "http://localhost:5173",
        supportFile: "cypress/support/e2e.js",
        setupNodeEvents(on, config){
            //we are going to use cy.task(): to write a tiny plugin
            on("task", {
                log(message) {
                    console.log(`[spec] ${message}`);
                    return null;
                }
            })
            registerCodeCoverage(on, config);
            getCompareSnapshotsPlugin(on, config);

            return config;
        }
    },
    component: {
        devServer: {
            framework: "react",
            bundler: "vite",
        }
    }
})