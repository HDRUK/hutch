import { Heading, SimpleGrid, VStack, Text } from "@chakra-ui/react";
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

      <SimpleGrid minChildWidth="400px" spacing={4}>
        {data.map((item, index) => (
          <ActivitySourceSummary
            key={index}
            href=""
            title={item.DisplayName}
            sourceURL={item.Host}
            collectionId={item.ResourceId}
          ></ActivitySourceSummary>
        ))}
      </SimpleGrid>
    </VStack>
  );
};
