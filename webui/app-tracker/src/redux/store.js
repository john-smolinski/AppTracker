import { configureStore } from "@reduxjs/toolkit";
import applicationReducer from "./applicationsSlice";
import jobTitleReducer from "./jobTitlesSlice";
import sourceReducer from "./sourcesSlice";
import workEnvrionmentReducer from "./workEnvironmentsSlice";
import organizationReducer from "./organizataionsSlice";

export const store = configureStore({
  reducer: {
    applications: applicationReducer,
    jobTitles: jobTitleReducer,
    appSources: sourceReducer,
    workEnvironments: workEnvrionmentReducer,
    organizations: organizationReducer,
  },
});

export default store;
