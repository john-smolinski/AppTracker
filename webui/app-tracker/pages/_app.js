import { Provider } from "react-redux";
import store from "@/redux/store";
import AppInitializer from "@/components/AppInitializer/AppInitializer";
import "@/styles/globals.css";

export default function App({ Component, pageProps }) {
  return (
    <Provider store={store}>
      <AppInitializer />
      <Component {...pageProps} />
    </Provider>
  );
}
