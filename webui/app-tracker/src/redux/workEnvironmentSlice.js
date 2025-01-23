import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import axios from "axios";

const API_BASE_URL = "http://localhost:5000/api/workenvironments";

// fetch Work Environments
export const fetchWorkEnvironments = createAsyncThunk(
  "workenvironments/fetchWorkEnvironments",
  async (_, { rejectWithValue }) => {
    try {
      const response = await axios.get(API_BASE_URL);
      return response.data;
    } catch (error) {
      return rejectWithValue({
        status: error.response?.status,
        message: error.response?.data || error.message,
      });
    }
  }
);

// Work Environment Slice
const workEnvironmentSlice = createSlice({
  name: "workEnvironments",
  initialState: {
    items: [],
    loading: false,
    error: null,
    isServerError: false,
  },
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch WorkEnvironments
      .addCase(fetchWorkEnvironments.pending, (state) => {
        state.loading = true;
        state.error = null;
        state.isServerError = false;
      })
      .addCase(fetchWorkEnvironments.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchWorkEnvironments.rejected, (state, action) => {
        state.loading = false;
        state.error = action.payload?.message || "An error occurred";
        state.isServerError = action.payload?.status === 500;
      });
  },
});

export default workEnvironmentSlice.reducer;
