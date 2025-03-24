import { useRouter } from "next/router";
import Application from "@/components/Pages/Applications/Application/Application";

export default function ApplicationPage() {
  const router = useRouter();
  const { id } = router.query;

  return <Application id={id} />;
}
