import { configureStore } from "@reduxjs/toolkit";
import applicationReducer from "./applicationSlice";
import jobTitleReducer from "./jobTitlesSlice";
import sourceReducer from "./sourcesSlice";
import workEnvrionmentReducer from "./workEnvironmentSlice";

export const store = configureStore({
  reducer: {
    applications: applicationReducer,
    jobTitles: jobTitleReducer,
    sources: sourceReducer,
    workEnvironments: workEnvrionmentReducer,
  },
});

export default store;
