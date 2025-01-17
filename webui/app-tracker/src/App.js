import "./App.css";
import ApplicationsList from "./components/ApplicationsList";
import Summary from "./components/Summary";
import Menu from "./components/Menu";

function App() {
  return (
    <div className="App">
      <Menu />
      <Summary />
      <ApplicationsList />
    </div>
  );
}

export default App;
