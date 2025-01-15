import { configureStore } from "@reduxjs/toolkit";
import applicationReducer from "./applicationSlice";

export const store = configureStore({
  reducer: {
    applications: applicationReducer,
  },
});

export default store;
