import { Heading, SimpleGrid, VStack } from "@chakra-ui/react";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <Heading as="h2" size="lg">
        {t("home.heading", { name: user?.fullName })}
      </Heading>

      <SimpleGrid minChildWidth="400px" spacing={4}>
        {/* <ActionCard
          href="process-overview"
          icon={AiFillRead}
          title="Proposals process overview"
        >
          <Text>
            Review the process for submitting proposals using this system
          </Text>
        </ActionCard> */}
      </SimpleGrid>

      <VStack w="100%" align="stretch">
        <Heading as="h3" size="lg">
          Collections
        </Heading>
      </VStack>
    </VStack>
  );
};
