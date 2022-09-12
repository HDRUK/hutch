import {
  Heading,
  VStack,
  Button,
  useDisclosure,
  Input,
  HStack,
  Text,
  InputGroup,
  InputLeftElement,
  Stack
} from "@chakra-ui/react";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { useSortingAndFiltering } from "helpers/hooks/useSortingAndFiltering";
import { useActivitySourceList } from "api/activitysources";
import { ActivitySourceSummary } from "components/activitysources/ActivitySourceSummary";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { DeleteModal } from "components/DeleteModal";
import { Link } from "react-router-dom";
import { FaPlus, FaSearch } from "react-icons/fa";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  const { isOpen, onOpen, onClose } = useDisclosure();
  const [selectedActivitySource, setSelectedActivitySource] = useState();
  const { activitysource } = useBackendApi();
  const { data, mutate } = useActivitySourceList();
  const {
    sorting,
    setSorting,
    onSort: handleSort,
    filter,
    setFilter,
    outputList,
  } = useSortingAndFiltering(data, "displayName", {
    initialSort: {
      key: "displayName",
    },
  });

  const onDeleteSource = async () => {
    await activitysource.delete({ id: selectedActivitySource.id });
    await mutate();
    onClose();
  };
  const onClickDelete = (activitySource) => {
    setSelectedActivitySource(activitySource);
    onOpen();
  };

  return (
    <Stack px={8} w="100%" spacing={4} p={4} alignItems='center'>
      <Heading as="h2" size="lg">
        {t("home.heading", { name: user?.fullName })}
      </Heading>
      <Stack w="100%" spacing={4} alignItems='center'>
        <Heading as="h3" size="md">
          Activity Sources
        </Heading>
        <HStack maxW={'600'}
          w={'100%'}
          alignSelf={'center'}
          borderRadius={'10px'}
        >
          <InputGroup>
            <InputLeftElement
              pointerEvents='none'
              children={<FaSearch color='gray.300' />}
            />
            <Input size={'md'}
              placeholder="Search Activity Sources"
              onChange={(e) => setFilter(e.target.value)}
            />
          </InputGroup>
          <Button
            as={Link}
            to="/activitysources/new"
            colorScheme='green'
            leftIcon={<FaPlus />}
          >
            <Text textTransform={'uppercase'}
              fontWeight={700}
              fontSize={'sm'}
              letterSpacing={1.1}> New</Text>
          </Button>
        </HStack>
        {outputList &&
          outputList.map((item, index) => (
            <ActivitySourceSummary
              key={index}
              href={`/activitysources/${item.id}`}
              onDelete={() => onClickDelete(item)}
              title={item.displayName}
              sourceURL={item.host}
              collectionId={item.resourceId}
            ></ActivitySourceSummary>
          ))}
      </Stack>
      <DeleteModal
        title={`Delete Activity Source?`}
        body={
          <VStack>
            <Text>Are you sure you want to delete this activity source:</Text>
            <Text fontWeight="bold">{selectedActivitySource?.displayName}</Text>
            <Text>You will not be able to reverse this!</Text>
          </VStack>
        }
        isOpen={isOpen}
        onClose={onClose}
        onDelete={onDeleteSource}
      />
    </Stack >
  );
};
