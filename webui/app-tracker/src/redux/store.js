import { configureStore } from "@reduxjs/toolkit";
import applicationReducer from "./applicationsSlice";
import jobTitleReducer from "./jobTitlesSlice";
import sourceReducer from "./sourcesSlice";
import workEnvrionmentReducer from "./workEnvironmentsSlice";

export const store = configureStore({
  reducer: {
    applications: applicationReducer,
    jobTitles: jobTitleReducer,
    sources: sourceReducer,
    workEnvironments: workEnvrionmentReducer,
  },
});

export default store;
