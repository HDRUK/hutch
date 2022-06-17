import {
  Heading,
  SimpleGrid,
  VStack,
  Text,
  Link,
  Button,
} from "@chakra-ui/react";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { ActionCard } from "components/ActionCard";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysource";
import { ActivitySourceSummary } from "components/ActivitySourceSummary";
//import { useEffect } from "react";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  const { data } = useActivitySourceList();

  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <Heading as="h2" size="lg">
        {t("home.heading", { name: user?.fullName })}
      </Heading>
      <VStack w="100%" align="stretch">
        <Heading as="h3" size="lg">
          Activity Sources
        </Heading>
        <Link href="/activitysources/new">
          <Button>New Activity Source</Button>
        </Link>
        {data.map((item, index) => (
          <ActivitySourceSummary
            key={index}
            href={`activitysources/${item.Id}`}
            title={item.DisplayName}
            sourceURL={item.Host}
            collectionId={item.ResourceId}
          ></ActivitySourceSummary>
        ))}
      </VStack>
    </VStack>
  );
};
