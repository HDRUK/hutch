import { Heading, VStack, Link, Button, useDisclosure } from "@chakra-ui/react";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { ActionCard } from "components/ActionCard";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysource";
import { ActivitySourceSummary } from "components/ActivitySourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteActivitySourceModal } from "components/DeleteActivitySourceModal";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedId, setSelectedId] = useState();
  const { activitysource } = useBackendApi();
  const { data } = useActivitySourceList();

  const onDeleteSource = async () => {
    await activitysource.delete({ id: selectedId });
    onClose();
  };
  const onClickDelete = (id) => {
    setSelectedId(id);
    onOpen();
  };

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
            onDelete={() => onClickDelete(item.Id)}
            title={item.DisplayName}
            sourceURL={item.Host}
            collectionId={item.ResourceId}
          ></ActivitySourceSummary>
        ))}
      </VStack>
      <DeleteActivitySourceModal
        isOpen={isOpen}
        onClose={onClose}
        id={selectedId}
        onDeleteSource={onDeleteSource}
      />
    </VStack>
  );
};
