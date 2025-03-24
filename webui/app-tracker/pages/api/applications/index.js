import axios from "axios";
import API_ROUTES from "@/config/apiConfig";

const BACKEND_API_URL = API_ROUTES.applications;

export default async function handler(req, res) {
  if (req.method === "GET") {
    try {
      const response = await axios.get(BACKEND_API_URL);
      res.status(200).json(response.data);
    } catch (error) {
      console.error("API Error", error);
      res.status(error.response?.status || 500).json({
        message: error.response?.data || "Internal server error",
      });
    }
  } else if (req.method === "POST") {
    try {
      const response = await axios.post(BACKEND_API_URL, req.body);
      res.status(201).json(response.data);
    } catch (error) {
      console.error("API Error", error);
      res.status(error.response?.data || 500).json({
        message: error.response?.data || "Error saving application",
      });
    }
  } else {
    res.status(405).json({ message: "Method Not Allowed" });
  }
}
