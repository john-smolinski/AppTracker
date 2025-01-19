import "../src/styles/app.css";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Home from "./components/Pages/Home/Home";
import SearchApplications from "./components/Pages/Search/SearchApplications";
import AddApplication from "./components/Pages/AddApplication/AddApplication";
import NotFound from "./components/Pages/NotFound/NotFound";
import Statistics from "./components/Pages/Statistics/Statistics";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/search" element={<SearchApplications />} />
        <Route path="/add" element={<AddApplication />} />
        <Route path="/statistics" element={<Statistics />} />
        <Route path="*" element={<NotFound />} />
      </Routes>
    </Router>
  );
}

export default App;
